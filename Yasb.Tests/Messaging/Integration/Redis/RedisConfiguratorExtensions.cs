using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging;
using Yasb.Wireup;
using Autofac;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Net;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Messaging.Connections;

namespace Yasb.Tests.Messaging.Integration.Redis
{
    public static class RedisConfiguratorExtensions
    {
        public static IRedisClient ConfigureRedisClient(this RedisConfigurator configurator, string host, int port = 6379)
        {
            var builder = new ContainerBuilder();
            var ipAddress = IPAddress.Parse(host);
            var endPoint = new RedisConnection(host, port);
            builder.RegisterWithScope<IRedisSocketAsyncEventArgsPool>((componentScope, parameters) =>
            {
                return new RedisSocketAsyncEventArgsPool(1, endPoint);
            });
            builder.RegisterOneInstanceForObjectKey<RedisConnection, IRedisClient>((connection, context) =>
            {

                var connectionsPool = context.Resolve<IRedisSocketAsyncEventArgsPool>(TypedParameter.From<RedisConnection>(connection));
                return new RedisClient(connectionsPool);
            });

            return builder.Build().BeginLifetimeScope("test").Resolve<IRedisClient>(TypedParameter.From<RedisConnection>(endPoint));
        }
    }
}
