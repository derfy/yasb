using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using System.Net;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Redis.Messaging;
using Yasb.Common.Messaging;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Builder;
using Yasb.Common.Serialization;
using Yasb.Redis.Messaging.Scripts;
using Newtonsoft.Json;
using Yasb.Common.Messaging.Connections;

namespace Yasb.Wireup
{
    public class RedisQueueModule : ScopedModule<QueueConfiguration<RedisConnection>>
    {
        public RedisQueueModule(QueueConfiguration<RedisConnection> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
        }
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterOneInstanceForObjectKey<RedisConnection, IRedisClient>((connection, context) =>
            {
                var connectionsPool = context.Resolve<IRedisSocketAsyncEventArgsPool>(TypedParameter.From<RedisConnection>(connection));
                return new RedisClient(connectionsPool); 
            });
            
           
            builder.RegisterWithScope<IRedisSocketAsyncEventArgsPool>((componentScope, parameters) =>
            {
                var endPoint = parameters.TypedAs<RedisConnection>();
                return new RedisSocketAsyncEventArgsPool(500, endPoint);
            });


            builder.RegisterWithScope<AbstractQueueFactory<RedisConnection>>((componentScope, parameters) =>
            {
                return new RedisQueueFactory(Configuration, componentScope.Resolve<ISerializer>(), componentScope.Resolve<RedisClientFactory>());
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisClientFactory>(componentScope =>
            {
                return endPoint => componentScope.Resolve<IRedisClient>(TypedParameter.From<RedisConnection>(endPoint));
            }).InstancePerMatchingLifetimeScope(Scope);

           
        }
    }
}
