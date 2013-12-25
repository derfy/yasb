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
using System.Collections.Concurrent;
using System.Net.Sockets;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Builder;
using Newtonsoft.Json;
using Yasb.Common.Messaging.EndPoints.Redis;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.Configuration.Redis;
using Yasb.Common.Serialization.Json;

namespace Yasb.Wireup
{

    public class RedisServiceBusModule : ServiceBusModule<RedisEndPoint, JsonNetSerializer<RedisEndPoint>>
    {
        public RedisServiceBusModule(ServiceBusConfiguration<RedisEndPoint, JsonNetSerializer<RedisEndPoint>> serviceBusConfiguration)
            : base(serviceBusConfiguration)
        {
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterOneInstanceForObjectKey<EndPoint, IRedisClient>((address, context) =>
            {
                var connectionsPool = context.Resolve<IRedisSocketAsyncEventArgsPool>(TypedParameter.From<EndPoint>(address));
                return new RedisClient(connectionsPool);
            });


            builder.RegisterWithScope<RedisSocketAsyncEventArgsPool>((componentScope, parameters) =>
            {
                var address = parameters.TypedAs<EndPoint>();
                return new RedisSocketAsyncEventArgsPool(10, address);
            }).As<IRedisSocketAsyncEventArgsPool>();

           

            builder.RegisterWithScope<RedisQueueFactory>((componentScope, parameters) =>
            {
                return new RedisQueueFactory(componentScope.Resolve<ISerializer>(), componentScope.Resolve<RedisClientFactory>());
            }).As<IQueueFactory<RedisEndPoint>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisClientFactory>(componentScope =>
            {
                return endPoint => componentScope.Resolve<IRedisClient>(TypedParameter.From<EndPoint>(endPoint.Address));
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisSubscriptionService>((componentScope, parameters) =>
            {
                var localEndPoint = Configuration.EndPoints["local"];
                return new RedisSubscriptionService(localEndPoint, componentScope.Resolve<IRedisClient>(TypedParameter.From<EndPoint>(localEndPoint.Address)), componentScope.Resolve<ISerializer>());
            }).As<ISubscriptionService<RedisEndPoint>>();
        }
    }
   
}
