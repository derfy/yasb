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
using Yasb.Common.Serialization.MessageDeserializers;
using Yasb.Common.Messaging.EndPoints.Redis;
using Yasb.Common.Messaging.Configuration.Redis;
using Yasb.Common.Serialization.Json;

namespace Yasb.Wireup
{
    public class RedisQueueModule : ScopedModule<ServiceBusConfiguration<RedisEndPoint, JsonNetSerializer<RedisEndPoint>>>
    {
        public RedisQueueModule(ServiceBusConfiguration<RedisEndPoint, JsonNetSerializer<RedisEndPoint>> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
        }
        protected override void Load(Autofac.ContainerBuilder builder)
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

            builder.RegisterWithScope<ISerializer>((componentScope, parameters) =>
            {
                return Configuration.Serializer;
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisQueueFactory>((componentScope, parameters) =>
            {
                return new RedisQueueFactory(componentScope.Resolve<ISerializer>(), componentScope.Resolve<RedisClientFactory>());
            }).As<IQueueFactory<RedisEndPoint>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisClientFactory>(componentScope =>
            {
                return endPoint => componentScope.Resolve<IRedisClient>(TypedParameter.From<EndPoint>(endPoint.Address));
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisQueue>(componentScope =>
            {
                var localEndPoint = Configuration.EndPoints["local"];
                var redisClientFactory = componentScope.Resolve<RedisClientFactory>();
                var redisClient = redisClientFactory(localEndPoint);
                return new RedisQueue(localEndPoint, componentScope.Resolve<ISerializer>(), redisClient);
              
            }).As<IQueue<RedisEndPoint>>().InstancePerMatchingLifetimeScope(Scope);


            builder.RegisterWithScope<Func<Type, IMessageDeserializer>>((componentScope) => type =>
            {
                if (!componentScope.IsRegisteredWithKey<IMessageDeserializer>(type))
                {
                    var genericType = typeof(DefaultMessageDeserializer<>).MakeGenericType(type);
                    return Activator.CreateInstance(genericType) as IMessageDeserializer;
                }
                return componentScope.ResolveKeyed<IMessageDeserializer>(type);
            }).InstancePerMatchingLifetimeScope(Scope);
            //builder.RegisterWithScope<MessageEnvelopeConverter>(componentScope => new MessageEnvelopeConverter(componentScope.Resolve<Func<Type, IMessageDeserializer>>())).As<JsonConverter>();
        }
    }
}
