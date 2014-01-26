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

namespace Yasb.Wireup
{

    public class RedisServiceBusModule : ServiceBusModule<RedisEndPointConfiguration, RedisSubscriptionServiceConfiguration>
    {
        public RedisServiceBusModule(ServiceBusConfiguration<RedisEndPointConfiguration, RedisSubscriptionServiceConfiguration> serviceBusConfiguration)
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
                return new RedisQueueFactory(componentScope.Resolve<AbstractJsonSerializer<MessageEnvelope>>(), componentScope.Resolve<RedisClientFactory>());
            }).As<IQueueFactory<RedisEndPointConfiguration>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisClientFactory>(componentScope =>
            {
                return address => componentScope.Resolve<IRedisClient>(TypedParameter.From<EndPoint>(address));
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<EndPointSerializer>((componentScope, parameters) =>
            {
                return new EndPointSerializer();
            }).As<AbstractJsonSerializer<RedisEndPointConfiguration>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<Func<Type, AbstractJsonSerializer<IMessage>>>((componentScope, parameters) =>
            {
                return (type) => new DefaultJsonMessageDeserializer(type);
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<JsonMessageEnvelopeSerializer>((componentScope, parameters) =>
            {
                return new JsonMessageEnvelopeSerializer(componentScope.Resolve<Func<Type, AbstractJsonSerializer<IMessage>>>());
            }).As<AbstractJsonSerializer<MessageEnvelope>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisSubscriptionService>((componentScope, parameters) =>
            {
                Configuration.SubscriptionServiceConfiguration.LocalEndPointConfiguration = Configuration.EndPoints["local"];
                return new RedisSubscriptionService(Configuration.SubscriptionServiceConfiguration, componentScope.Resolve<RedisClientFactory>(), componentScope.Resolve<AbstractJsonSerializer<RedisEndPointConfiguration>>());
            }).InstancePerMatchingLifetimeScope(Scope).As<ISubscriptionService<RedisEndPointConfiguration>>();
        }
    }
   
}
