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

            builder.Register<RedisSocket>(c => new RedisSocket(c.Resolve<Func<AddressInfo, ConcurrentQueue<RedisSocketAsyncEventArgs>>>())).As<RedisSocket>().SingleInstance();


            for (int i = 0; i < _configuration.AddressInfoList.Length; i++)
            {
                var addressInfo = _configuration.AddressInfoList[i];
                var connectionsQueue = CreateConnectionQueue(addressInfo);

                builder.Register<RedisClient>(c => new RedisClient(c.Resolve<RedisSocket>(), addressInfo)).Keyed<RedisClient>(addressInfo);

                builder.RegisterInstance(connectionsQueue).As<ConcurrentQueue<RedisSocketAsyncEventArgs>>()
                    .Keyed<ConcurrentQueue<RedisSocketAsyncEventArgs>>(addressInfo).SingleInstance();
            }
            builder.Register<Func<AddressInfo, ConcurrentQueue<RedisSocketAsyncEventArgs>>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return (addressInfo) => context.ResolveKeyed<ConcurrentQueue<RedisSocketAsyncEventArgs>>(addressInfo);
            }).As<Func<AddressInfo, ConcurrentQueue<RedisSocketAsyncEventArgs>>>().SingleInstance();

            

            builder.Register<Func<AddressInfo, RedisClient>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return (addressInfo) => context.ResolveKeyed<RedisClient>(addressInfo);
            }).As<Func<AddressInfo, RedisClient>>().SingleInstance();

          

            for (int i = 0; i < _configuration.EndPoints.Length; i++)
            {
                var endPoint = _configuration.EndPoints[i];
                builder.Register(c => new Queue(endPoint, c.Resolve<ISerializer>(), c.Resolve<Func<AddressInfo, RedisClient>>())).As<IQueue>()
                    .Keyed<IQueue>(endPoint);
            }

            builder.Register<Func<BusEndPoint, IQueue>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return endPoint => context.ResolveKeyed<IQueue>(endPoint);

            }).As<Func<BusEndPoint, IQueue>>();

            
            builder.Register<Func<Type, IEnumerable<IHandleMessages>>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return type =>
                {
                    var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                    var collType = typeof(IEnumerable<>).MakeGenericType(genericType);
                    var resolved = context.IsRegistered(genericType);
                    return context.Resolve(collType) as IEnumerable<IHandleMessages>;
                };
            });

            builder.Register<ISubscriptionService>(c => new SubscriptionService(localEndPoint, c.Resolve<Func<AddressInfo, RedisClient>>()));

            builder.RegisterType<TaskRunner>().As<ITaskRunner>();
            builder.Register(c => new MessagesSender(c.Resolve<Func<BusEndPoint,IQueue>>()))
                   .As<IMessagesSender>();

            builder.Register(c => new MessagesReceiver(c.ResolveKeyed<IQueue>(localEndPoint), c.Resolve<Func<Type, IEnumerable<IHandleMessages>>>())).As(typeof(IWorker));

            builder.Register(c => new SubscriptionMessageHandler(c.Resolve<ISubscriptionService>())).As<IHandleMessages<SubscriptionMessage>>();
            builder.Register(c => new ServiceBus(_configuration, c.Resolve<IWorker>(), c.Resolve<IMessagesSender>(), c.Resolve<ISubscriptionService>(), c.Resolve<ITaskRunner>())).As<IServiceBus>();

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
