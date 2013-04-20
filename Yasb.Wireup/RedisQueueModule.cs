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
            builder.RegisterOneInstanceForObjectKey<EndPoint, IRedisClient>((connection, context) => new RedisClient(context.Resolve<RedisSocket>(TypedParameter.From<EndPoint>(connection))));

            builder.RegisterWithScope<RedisSocket>((componentScope, parameters) =>
            {
                return new RedisSocket(componentScope.Resolve<IRedisSocketAsyncEventArgsPool>(parameters));
            }).As(typeof(RedisSocket));

            builder.RegisterWithScope<IRedisSocketAsyncEventArgsPool>((componentScope, parameters) =>
            {
                var endPoint = parameters.TypedAs<EndPoint>();
                return new RedisSocketAsyncEventArgsPool(10, endPoint);
            });

            builder.RegisterWithScope<ScriptsCache>((componentScope, parameters) =>
            {
                return new ScriptsCache(componentScope.Resolve<IRedisClient>(parameters));
            });



            builder.RegisterWithScope<IQueue>((componentScope, parameters) =>
            {
                var endPoint = parameters.Named<BusEndPoint>("endPoint");
                var connection = Configuration.GetConnectionByName(endPoint.ConnectionName);
                var redisClient = componentScope.Resolve<IRedisClient>(TypedParameter.From<EndPoint>(connection));
                var scriptsCache = new ScriptsCache(redisClient);
                scriptsCache.EnsureScriptCached("TryGetEnvelope.lua");
                scriptsCache.EnsureScriptCached("SetMessageCompleted.lua");
                return new RedisQueue(endPoint.QueueName, componentScope.Resolve<ISerializer>(),redisClient, scriptsCache);
            });
        }
    }
}
