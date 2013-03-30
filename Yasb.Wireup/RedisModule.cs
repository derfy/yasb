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
        private ServiceBusConfiguration<RedisEndPoint, RedisEndPointConfiguration> _configuration;

        public RedisModule(ServiceBusConfiguration<RedisEndPoint, RedisEndPointConfiguration> configuration)
        {
            this._configuration = configuration;
        }
        protected override void Load(ContainerBuilder builder)
        {

            var localEndPoint = _configuration.LocalEndPoint;
          

            builder.RegisterWithScope<RedisSocket>((componentScope, parameters) =>
            {
                return new RedisSocket(componentScope.Resolve<IRedisSocketAsyncEventArgsPool>(parameters));
            }).As(typeof(RedisSocket)).InstancePerLifetimeScope(); ;

            builder.RegisterWithScope<RedisClient>((componentScope, parameters) =>
            {
                return new RedisClient(componentScope.Resolve<RedisSocket>(parameters));
            }).As(typeof(RedisClient)).InstancePerLifetimeScope(); ;

            builder.RegisterWithScope<IQueue>((componentScope, parameters) =>
            {
                var endPoint = parameters.OfType<TypedParameter>().First().Value as RedisEndPoint;
                return new Queue(endPoint, componentScope.Resolve<ISerializer>(), componentScope.Resolve<RedisClient>(TypedParameter.From<EndPoint>(endPoint.ToIPEndPoint())));
            }).As(typeof(IQueue));

            builder.RegisterWithScope<ISubscriptionService>(componentScope =>
            {
                return new SubscriptionService(localEndPoint, componentScope.Resolve<RedisClient>(TypedParameter.From<EndPoint>(localEndPoint.ToIPEndPoint())));
            }).As(typeof(ISubscriptionService));

            builder.RegisterWithScope<IHandleMessages>((componentScope, p) =>
            {
                var type = p.OfType<TypedParameter>().First().Value as Type;
                var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                return componentScope.Resolve(genericType) as IHandleMessages;
            });

           

            builder.RegisterWithScope<IWorker>(componentScope =>
            {
                var localQueue = componentScope.Resolve<IQueue>(TypedParameter.From<IEndPoint>(localEndPoint));
                return new MessagesReceiver(localQueue, componentScope.Resolve<Func<Type, IEnumerable<IHandleMessages>>>());
            }).As(typeof(IWorker)).InstancePerLifetimeScope();

            

            builder.RegisterType<Serializer>().WithParameter(TypedParameter.From<JsonConverter[]>(new JsonConverter[] { new RedisEndPointConverter(), new MessageEnvelopeConverter<RedisEndPoint>() })).As<ISerializer>();
            builder.RegisterType<TaskRunner>().As<ITaskRunner>().InstancePerLifetimeScope();
            builder.RegisterType<MessagesSender<RedisEndPoint>>().As<IMessagesSender<RedisEndPoint>>().InstancePerLifetimeScope();
            builder.RegisterType<SubscriptionMessageHandler<RedisEndPoint>>().As<IHandleMessages<SubscriptionMessage<RedisEndPoint>>>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceBus<RedisEndPoint>>().WithParameter(TypedParameter.From<RedisEndPoint[]>(_configuration.NamedEndPoints)).As<IServiceBus<RedisEndPoint>>();
                
       
            if (_configuration.MessageHandlersAssembly != null)
            {
                builder.RegisterAssemblyTypes(_configuration.MessageHandlersAssembly)
                       .AsClosedTypesOf(typeof(IHandleMessages<>))
                       .AsImplementedInterfaces();
            }
           

            
        }
       

        


    }
}
