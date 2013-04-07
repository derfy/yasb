using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using System.Net;
using System.Reflection;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Common;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging.Configuration;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Builder;
using Yasb.Redis.Messaging.Configuration;
using Newtonsoft.Json;
using Yasb.Redis.Messaging.Serialization;

namespace Yasb.Wireup
{
    public class RedisModule : Autofac.Module
    {
        private ServiceBusConfiguration _configuration;

        public RedisModule(ServiceBusConfiguration configuration)
        {
            this._configuration = configuration;
        }
        protected override void Load(ContainerBuilder builder)
        {

            var localEndPoint = _configuration.LocalEndPoint;


            builder.RegisterWithScope<RedisSocket>((componentScope, parameters) =>
            {
                 return new RedisSocket(componentScope.Resolve<IRedisSocketAsyncEventArgsPool>(parameters));
            }).As(typeof(RedisSocket));

            builder.RegisterWithScope<IRedisSocketAsyncEventArgsPool>((componentScope, parameters) =>
            {
                var endPoint = parameters.OfType<TypedParameter>().First().Value as EndPoint;
                return new RedisSocketAsyncEventArgsPool(10, endPoint);
            });

            builder.RegisterWithScope<ScriptsCache>((componentScope, parameters) =>
            {
                var endPoint = parameters.OfType<TypedParameter>().First().Value as RedisEndPoint;
                return new ScriptsCache(componentScope.Resolve<RedisClient>(TypedParameter.From<EndPoint>(endPoint.ToIPEndPoint())));
            });

            builder.RegisterWithScope<IQueue>((componentScope, parameters) =>
            {
                var endPoint = parameters.OfType<TypedParameter>().First().Value as RedisEndPoint;
                return new RedisQueue(endPoint, componentScope.Resolve<ISerializer>(), componentScope.Resolve<ScriptsCache>(parameters));
            }).As(typeof(IQueue));

            builder.RegisterWithScope<ISubscriptionService>((componentScope, parameters) =>
            {
                var redisEndPoint = localEndPoint as RedisEndPoint;
                if (redisEndPoint == null)
                    throw new ApplicationException("No valid Local endPoint has been provided");
                return new SubscriptionService(localEndPoint, componentScope.Resolve<RedisClient>(TypedParameter.From<EndPoint>(redisEndPoint.ToIPEndPoint())));
            }).InstancePerMatchingLifetimeScope("bus");;

            builder.RegisterWithScope<IHandleMessages>((componentScope, p) =>
            {
                var type = p.OfType<TypedParameter>().First().Value as Type;
                var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                return componentScope.Resolve(genericType) as IHandleMessages;
            }).InstancePerMatchingLifetimeScope("bus");;

           

            builder.RegisterWithScope<IWorker>(componentScope =>
            {
                var localQueue = componentScope.Resolve<IQueue>(TypedParameter.From<IEndPoint>(localEndPoint));
                return new MessagesReceiver(localQueue, componentScope.Resolve<Func<Type, IEnumerable<IHandleMessages>>>());
            }).As(typeof(IWorker)).InstancePerMatchingLifetimeScope("bus");

            builder.RegisterWithScope<ISerializer>(componentScope => new Serializer(new JsonConverter[] { new RedisEndPointConverter(), new MessageEnvelopeConverter<RedisEndPoint>() })).InstancePerMatchingLifetimeScope("bus");

            builder.RegisterWithScope<ITaskRunner>(componentScope => new TaskRunner()).As<ITaskRunner>().InstancePerMatchingLifetimeScope("bus");
            builder.RegisterWithScope<IHandleMessages<SubscriptionMessage>>(componentScope => new SubscriptionMessageHandler(componentScope.Resolve<ISubscriptionService>())).As<IHandleMessages<SubscriptionMessage>>().InstancePerMatchingLifetimeScope("bus");
            builder.RegisterWithScope<Func<IEndPoint, IQueue>>(componentScope => (endPoint)=>componentScope.Resolve<IQueue>(TypedParameter.From<IEndPoint>(endPoint)),"queueFactory" ).InstancePerMatchingLifetimeScope("bus");

            builder.RegisterWithScope<IServiceBus>(componentScope => new ServiceBus(_configuration.NamedEndPoints, componentScope.Resolve<IWorker>(), componentScope.Resolve<Func<IEndPoint, IQueue>>(), componentScope.Resolve<ISubscriptionService>(), componentScope.Resolve<ITaskRunner>())).As<IServiceBus>().InstancePerMatchingLifetimeScope("bus");

                
       
            if (_configuration.MessageHandlersAssembly != null)
            {
                builder.RegisterAssemblyTypes(_configuration.MessageHandlersAssembly)
                       .AsClosedTypesOf(typeof(IHandleMessages<>))
                       .AsImplementedInterfaces();
            }
           

            
        }
       

        


    }
}
