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
using Yasb.Common.Serialization;
using Yasb.Redis.Messaging.Scripts;

namespace Yasb.Wireup
{
    public class RedisQueueModule : CommonModule<ConnectionsRepository<EndPoint>>
    {
        public RedisQueueModule(ConnectionsRepository<EndPoint> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
        }
       

        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterOneInstanceForObjectKey<EndPoint, IRedisClient>((connection, context) => new RedisClient(context.Resolve<RedisConnectionManager>(TypedParameter.From<EndPoint>(connection))));

            builder.RegisterWithScope<RedisConnectionManager>((componentScope, parameters) =>
            {
                return new RedisConnectionManager(componentScope.Resolve<IRedisSocketAsyncEventArgsPool>(parameters));
            }).As(typeof(RedisConnectionManager));

            builder.RegisterWithScope<IRedisSocketAsyncEventArgsPool>((componentScope, parameters) =>
            {
                var endPoint = parameters.TypedAs<EndPoint>();
                return new RedisSocketAsyncEventArgsPool(3, endPoint);
            });

            builder.RegisterWithScope<ScriptsCache>((componentScope, parameters) =>
            {
                var scriptsCache= new ScriptsCache(componentScope.Resolve<IRedisClient>(parameters));
                scriptsCache.EnsureScriptCached("TryGetEnvelope.lua");
                scriptsCache.EnsureScriptCached("SetMessageCompleted.lua");
                return scriptsCache;
            }).InstancePerMatchingLifetimeScope(Scope);



            builder.RegisterWithScope<IQueue>((componentScope, parameters) =>
            {
                var endPoint = parameters.Named<BusEndPoint>("endPoint");
                var connection = Configuration.GetConnectionByName(endPoint.ConnectionName);
                var redisClient = componentScope.Resolve<IRedisClient>(TypedParameter.From<EndPoint>(connection));
                var scriptsCache = componentScope.Resolve<ScriptsCache>(TypedParameter.From<EndPoint>(connection)); //new ScriptsCache(redisClient);
               
                return new RedisQueue(endPoint.QueueName, componentScope.Resolve<ISerializer>(),redisClient, scriptsCache);
            });
        }
    }
}
