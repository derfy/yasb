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
using Yasb.Common.Messaging.Tcp;

namespace Yasb.Wireup
{
    public class RedisServiceBusModule : ServiceBusModule<RedisEndPoint,RedisEndPointConfiguration>
    {
        public RedisServiceBusModule(EndPointsConfigurer<RedisEndPointConfiguration> configuration):base(configuration)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<RedisClientFactory>(componentScope =>
            {
                return address => componentScope.Resolve<IRedisClient>(TypedParameter.From<EndPoint>(address));
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterOneInstanceForObjectKey<EndPoint, IRedisClient>((address, context) =>
            {
                var connectionsPool = context.Resolve<ITcpConnectionsPool<RedisConnection>>(TypedParameter.From<EndPoint>(address));
                var connectionManager = new TcpConnectionsManager<RedisConnection>(connectionsPool);
                return new RedisClient(connectionManager);
            });


            builder.RegisterWithScope<TcpConnectionsPool<RedisConnection>>((componentScope, parameters) =>
            {
                var address = parameters.TypedAs<EndPoint>();
                return new TcpConnectionsPool<RedisConnection>(10, ()=> new RedisConnection(address));
            }).As<ITcpConnectionsPool<RedisConnection>>();



            builder.RegisterWithScope<RedisQueueFactory>((componentScope, parameters) =>
            {
                return new RedisQueueFactory(componentScope.Resolve<AbstractJsonSerializer<MessageEnvelope>>(), componentScope.Resolve<RedisClientFactory>());
            }).As<IQueueFactory<RedisEndPoint>>().InstancePerMatchingLifetimeScope(Scope);

           
            builder.RegisterWithScope<Func<Type, AbstractJsonSerializer<IMessage>>>((componentScope, parameters) =>
            {
                return (type) => new DefaultJsonMessageDeserializer(type);
            }).InstancePerMatchingLifetimeScope(Scope);
            builder.RegisterWithScope<JsonMessageEnvelopeSerializer>((componentScope, parameters) =>
            {
                return new JsonMessageEnvelopeSerializer(componentScope.Resolve<Func<Type, AbstractJsonSerializer<IMessage>>>());
            }).As<AbstractJsonSerializer<MessageEnvelope>>().InstancePerMatchingLifetimeScope(Scope);

        }

    }
   
}
