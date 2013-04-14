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
using Yasb.Common.Serialization;
using Yasb.Common.Messaging.Configuration;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Builder;
using Yasb.Redis.Messaging.Configuration;
using Newtonsoft.Json;

namespace Yasb.Wireup
{
    public class RedisModule : Autofac.Module
    {

        private ServiceBusConfiguration<EndPoint> _serviceBusConfiguration;
        public RedisModule(ServiceBusConfiguration<EndPoint> serviceBusConfiguration)
        {
            _serviceBusConfiguration = serviceBusConfiguration;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterOneInstanceForObjectKey<EndPoint, RedisClient>((connection, context) => new RedisClient(context.Resolve<RedisSocket>(TypedParameter.From<EndPoint>(connection))));
            
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
                return new ScriptsCache(componentScope.Resolve<RedisClient>(parameters));
            });

            

            builder.RegisterWithScope<IQueue>((componentScope, parameters) =>
            {
                var endPoint = parameters.Named<BusEndPoint>("endPoint");
                var connection = _serviceBusConfiguration.GetConnectionByName(endPoint.ConnectionName);
                return new RedisQueue(endPoint, componentScope.Resolve<ISerializer>(), componentScope.Resolve<ScriptsCache>(TypedParameter.From<EndPoint>(connection)));
            });
          

           

            builder.RegisterWithScope<ISubscriptionService>((componentScope, parameters) =>
            {
                var localEndPoint = _serviceBusConfiguration.EndPointConfiguration.LocalEndPoint;
                var connection = _serviceBusConfiguration.GetConnectionByName(localEndPoint.ConnectionName);
                return new SubscriptionService(localEndPoint, componentScope.Resolve<RedisClient>(TypedParameter.From<EndPoint>(connection)));
            }).InstancePerMatchingLifetimeScope("bus"); 
           
            
        }
    }
}
