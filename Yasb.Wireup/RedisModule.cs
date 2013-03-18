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

            builder.RegisterType<RedisSocket>().SingleInstance();

            builder.Register<Func<AddressInfo, ConcurrentQueue<RedisSocketAsyncEventArgs>>>(ctx => {
                var lifetimeScopeFactory = new LifetimeScopeFactory(ctx.Resolve<ILifetimeScope>(), (tag, cb) => cb.Register<ConcurrentQueue<RedisSocketAsyncEventArgs>>(c => CreateConnectionQueue((AddressInfo)tag)).InstancePerMatchingLifetimeScope(tag));
                _configuration.AddressInfoList.ToList().ForEach(addressInfo => lifetimeScopeFactory.EnsureLifetimeScopeFor(addressInfo));
                return (addressInfo)=>{
                    lifetimeScopeFactory.EnsureLifetimeScopeFor(addressInfo);
                    var addressScope=lifetimeScopeFactory.GetLifetimeScopeFor(addressInfo);
                    return addressScope.Resolve<ConcurrentQueue<RedisSocketAsyncEventArgs>>();
                };
            }).SingleInstance();

            _configuration.AddressInfoList.ToList().ForEach(addressInfo => builder.RegisterType<RedisClient>().WithParameter(new TypedParameter(typeof(AddressInfo), addressInfo)));
           
            builder.Register<Func<AddressInfo, RedisClient>>(c => {
                var context = c.Resolve<IComponentContext>();
                return addressInfo =>
                {
                    return context.Resolve<RedisClient>(new TypedParameter(typeof(AddressInfo), addressInfo));
                };
            }).SingleInstance();

            _configuration.EndPoints.ToList().ForEach(endPoint => builder.RegisterType<Queue>().As<IQueue>().WithParameter(new TypedParameter(typeof(BusEndPoint), endPoint)));
           
              
            builder.Register<Func<BusEndPoint, IQueue>>(c =>
            {
                var lifetimeScope = c.Resolve<ILifetimeScope>();
                return endPoint =>
                {
                    using (var queueScope = lifetimeScope.BeginLifetimeScope())
                    {
                        return queueScope.Resolve<IQueue>(new TypedParameter(typeof(BusEndPoint), endPoint));
                    }
                    
                };
            });
           
            
            builder.Register<Func<Type, IEnumerable<IHandleMessages>>>(c =>
            {
                var lifetimeScope = c.Resolve<ILifetimeScope>();
                return type =>
                {
                    using (var currentScope = lifetimeScope.BeginLifetimeScope())
                    {
                        var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                        var collType = typeof(IEnumerable<>).MakeGenericType(genericType);
                        return currentScope.Resolve(collType) as IEnumerable<IHandleMessages>;
                    }
                };
            });

            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().WithParameter(new TypedParameter(typeof(BusEndPoint),localEndPoint));

            builder.RegisterType<TaskRunner>().As<ITaskRunner>();
            builder.RegisterType<MessagesSender>().As<IMessagesSender>();

            builder.Register(c => {
                var lifetimeScope = c.Resolve<ILifetimeScope>();
                using (var currentScope = lifetimeScope.BeginLifetimeScope())
                {
                    var localQueue = c.Resolve<Func<BusEndPoint, IQueue>>()(localEndPoint);
                    return new MessagesReceiver(localQueue, c.Resolve<Func<Type, IEnumerable<IHandleMessages>>>());
                }
            }).As(typeof(IWorker));

            builder.RegisterType<SubscriptionMessageHandler>().As<IHandleMessages<SubscriptionMessage>>();
            builder.RegisterType<ServiceBus>().WithParameter(new TypedParameter(typeof(IServiceBusConfiguration),_configuration)).As<IServiceBus>();
                
       
            if (_configuration.MessageHandlersAssembly != null)
            {
                builder.RegisterAssemblyTypes(_configuration.MessageHandlersAssembly)
                       .AsClosedTypesOf(typeof(IHandleMessages<>))
                       .AsImplementedInterfaces();
            }
           
        }

       
        private static ConcurrentQueue<RedisSocketAsyncEventArgs> CreateConnectionQueue(AddressInfo addressInfo)
        {
            var connectionsQueue = new ConcurrentQueue<RedisSocketAsyncEventArgs>();
            for (int ii = 0; ii < 10; ii++)
            {
                var connectEventArg = new RedisSocketAsyncEventArgs()
                {
                    RemoteEndPoint = addressInfo.ToEndPoint(),
                    AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                };
                connectionsQueue.Enqueue(connectEventArg);
            }
            return connectionsQueue;
        }

       

        


    }
}
