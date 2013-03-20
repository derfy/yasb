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

namespace Yasb.Wireup
{
    public class RedisModule : Autofac.Module
    {
        private ServiceBusConfiguration _configuration;
        private ConcurrentDictionary<AddressInfo, AddressInfo> _addressInfoSet = new ConcurrentDictionary<AddressInfo, AddressInfo>();
        private object _locker=new object();
        public RedisModule(ServiceBusConfiguration configuration)
        {
            this._configuration = configuration;
        }
        protected override void Load(ContainerBuilder builder)
        {

            var localEndPoint = _configuration.LocalEndPoint;
            builder.RegisterType<ConcurrentQueueRedisSocketEventArgsPool>().As<IRedisSocketAsyncEventArgsPool>().SingleInstance();
            builder.RegisterType<RedisClient>().InstancePerLifetimeScope();
            builder.RegisterWithScope<RedisSocket>(componentScope =>
            {
                return new RedisSocket(componentScope.Resolve<IRedisSocketAsyncEventArgsPool>(TypedParameter.From<int>(10)));
            }).InstancePerLifetimeScope();

            

            builder.RegisterWithScope<IQueue>((componentScope,parameters) =>
            {
                var endPoint = parameters.OfType<TypedParameter>().First().Value as BusEndPoint;
                return new Queue(endPoint, componentScope.Resolve<ISerializer>(), componentScope.Resolve<RedisClient>(TypedParameter.From<AddressInfo>(endPoint.AddressInfo)));
            }).As(typeof(IQueue));

            builder.RegisterWithScope<ISubscriptionService>(componentScope =>
            {
                return new SubscriptionService(localEndPoint, componentScope.Resolve<RedisClient>(TypedParameter.From<AddressInfo>(localEndPoint.AddressInfo)));
            }).As(typeof(ISubscriptionService));

            

            builder.RegisterWithScope<IEnumerable<IHandleMessages>>((componentScope, p) =>
            {
                var type = p.OfType<TypedParameter>().First().Value as Type;
                var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                var collType = typeof(IEnumerable<>).MakeGenericType(genericType);
                return componentScope.Resolve(collType) as IEnumerable<IHandleMessages>;
            });

            builder.RegisterWithScope<IWorker>(componentScope =>
            {
                var localQueue = componentScope.Resolve<IQueue>(TypedParameter.From<BusEndPoint>(localEndPoint));
                return new MessagesReceiver(localQueue, componentScope.Resolve<Func<Type, IEnumerable<IHandleMessages>>>());
            }).As(typeof(IWorker)).InstancePerLifetimeScope();

            builder.RegisterWithScope<Func<Type, IEnumerable<IHandleMessages>>>(componentScope =>
            {
                return type => componentScope.Resolve<IEnumerable<IHandleMessages>>(TypedParameter.From<Type>(type));
            }).InstancePerLifetimeScope();


            builder.RegisterType<TaskRunner>().As<ITaskRunner>().InstancePerLifetimeScope();
            builder.RegisterType<MessagesSender>().As<IMessagesSender>().InstancePerLifetimeScope();



            builder.RegisterType<SubscriptionMessageHandler>().As<IHandleMessages<SubscriptionMessage>>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceBus>().WithParameter(TypedParameter.From<IServiceBusConfiguration>(_configuration)).As<IServiceBus>();
                
       
            if (_configuration.MessageHandlersAssembly != null)
            {
                builder.RegisterAssemblyTypes(_configuration.MessageHandlersAssembly)
                       .AsClosedTypesOf(typeof(IHandleMessages<>))
                       .AsImplementedInterfaces();
            }
           

            
        }
       

        


    }
}
