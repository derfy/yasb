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

namespace Yasb.Tests.Messaging.Integration.Redis
{
    public static class RedisConfiguratorExtensions
    {
        public static IRedisClient ConfigureRedisClient(this RedisConfigurator configurator, string host, int port = 6379)
        {
            var builder = new ContainerBuilder();
            var ipAddress = IPAddress.Parse(host);
            var endPoint = new IPEndPoint(ipAddress, port);
            builder.RegisterWithScope<IRedisSocketAsyncEventArgsPool>((componentScope, parameters) =>
            {
                return new RedisSocketAsyncEventArgsPool(1, endPoint);
            });
            builder.RegisterOneInstanceForObjectKey<EndPoint, IRedisClient>((connection, context) =>
            {
                
                var connectionManager = new RedisConnectionManager(context.Resolve<IRedisSocketAsyncEventArgsPool>(TypedParameter.From<EndPoint>(connection)));
                return new RedisClient(connectionManager);
            });
           
            return builder.Build().BeginLifetimeScope("test").Resolve<IRedisClient>(TypedParameter.From<EndPoint>(endPoint));
        }
    }
}
