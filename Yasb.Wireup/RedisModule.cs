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
            var localEndPoint = _configuration.EndPoint;
            var redisClient = CreateRedisClient(localEndPoint);
            builder.RegisterInstance<RedisClient>(redisClient).SingleInstance();
            builder.Register(c => new Queue(c.Resolve<RedisClient>(), c.Resolve<ISerializer>(), localEndPoint.QueueName)).As<IQueue>();
            builder.Register<Func<Type, IEnumerable<IHandleMessages>>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return type =>
                {
                    var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                    var collType = typeof(IEnumerable<>).MakeGenericType(genericType);
                    return context.Resolve(collType) as IEnumerable<IHandleMessages>;
                };
            });
            builder.Register<Func<BusEndPoint, IQueue>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return endPoint =>
                {
                    var client = CreateRedisClient(endPoint);
                    return new Queue(client,context.Resolve<ISerializer>(),endPoint.QueueName);
                };
            });
            builder.RegisterType<TaskRunner>().As<ITaskRunner>();
            builder.Register(c => new RedisSubscriptionService(localEndPoint, redisClient)).As<ISubscriptionService>();
            builder.Register(c => new MessagesSender(c.Resolve<Func<BusEndPoint,IQueue>>()))
                   .As<IMessagesSender>();
           
            builder.Register(c => new MessagesReceiver(c.Resolve<IQueue>(), c.Resolve<Func<Type, IEnumerable<IHandleMessages>>>()))
                   .As(typeof(IWorker));

            builder.Register(c => new SubscriptionMessageHandler(c.Resolve<ISubscriptionService>()))
                  .As<IHandleMessages<SubscriptionMessage>>();
            builder.Register(c => new ServiceBus(localEndPoint, c.Resolve<IWorker>(), c.Resolve<IMessagesSender>(), c.Resolve<ISubscriptionService>(), c.Resolve<ITaskRunner>())).As<IServiceBus>();

            if (_configuration.MessageHandlersAssembly != null)
            {
                builder.RegisterAssemblyTypes(_configuration.MessageHandlersAssembly)
                       .AsClosedTypesOf(typeof(IHandleMessages<>))
                       .AsImplementedInterfaces();
            }
        }

        private static RedisClient CreateRedisClient(BusEndPoint endPoint)
        {
            var ipAddress = IPAddress.Parse(endPoint.Host);
            var socketClient = new RedisSocket(new System.Net.IPEndPoint(ipAddress, endPoint.Port));
            var redisCient = new RedisClient(socketClient);
            return redisCient;
        }

    }
}
