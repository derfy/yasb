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
using System.Collections.Concurrent;
using System.Net.Sockets;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Builder;
using Newtonsoft.Json;
using Yasb.Common.Messaging.Configuration;
using Yasb.Redis.Messaging.Serialization;
using Yasb.Redis.Messaging.Configuration;
using Yasb.Common.Messaging.Serialization;
using Yasb.Common.Messaging.Serialization.Json;
using Yasb.Redis.Messaging.Serialization.MessageDeserializers;
using Yasb.Msmq.Messaging;

namespace Yasb.Wireup
{

    public class RedisSubscriptionServiceModule<TEndPoint> : ScopedModule<SubscriptionServiceConfiguration>
    {
        public RedisSubscriptionServiceModule(SubscriptionServiceConfiguration configuration)
            : base(configuration, "bus")
        {

        }
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterWithScope<EndPointSerializer>((componentScope, parameters) =>
            {
                return new EndPointSerializer();
            }).As<AbstractJsonSerializer<RedisEndPoint>>().InstancePerMatchingLifetimeScope(Scope);



            builder.RegisterWithScope<RedisSubscriptionService<TEndPoint>>((componentScope, parameters) =>
            {
                var endPointFactory = componentScope.Resolve<Func<string, TEndPoint>>();
                var localEndPoint = endPointFactory("local");
                var redisClientFactory = componentScope.Resolve<RedisClientFactory>();
                var redisClient = redisClientFactory(Configuration.ServerAddress);
                return new RedisSubscriptionService<TEndPoint>(localEndPoint, redisClient, componentScope.Resolve<AbstractJsonSerializer<TEndPoint>>());
            }).InstancePerMatchingLifetimeScope(Scope).As<ISubscriptionService<TEndPoint>>();
        }
    }
   
}
