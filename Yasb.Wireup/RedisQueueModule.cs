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
using Yasb.Common.Serialization.MessageDeserializers;

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
            
           
            builder.RegisterWithScope<RedisSocketAsyncEventArgsPool>((componentScope, parameters) =>
            {
                var endPoint = parameters.TypedAs<RedisConnection>();
                return new RedisSocketAsyncEventArgsPool(10, endPoint);
            }).As<IRedisSocketAsyncEventArgsPool>();


            builder.RegisterWithScope<RedisQueueFactory>((componentScope, parameters) =>
            {
                return new RedisQueueFactory(Configuration, componentScope.Resolve<ISerializer>(), componentScope.Resolve<RedisClientFactory>());
            }).As<AbstractQueueFactory<RedisConnection>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisClientFactory>(componentScope =>
            {
                return endPoint => componentScope.Resolve<IRedisClient>(TypedParameter.From<RedisConnection>(endPoint));
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<Func<Type, IMessageDeserializer>>((componentScope) => type =>
            {
                if (!componentScope.IsRegisteredWithKey<IMessageDeserializer>(type))
                {
                    var genericType = typeof(DefaultMessageDeserializer<>).MakeGenericType(type);
                    return Activator.CreateInstance(genericType) as IMessageDeserializer;
                }
                return componentScope.ResolveKeyed<IMessageDeserializer>(type);
            }).InstancePerMatchingLifetimeScope(Scope);
            builder.RegisterWithScope<MessageEnvelopeConverter>(componentScope => new MessageEnvelopeConverter(componentScope.Resolve<Func<Type, IMessageDeserializer>>())).As<JsonConverter>();
        }
    }
}
