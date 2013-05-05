﻿using System;
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

namespace Yasb.Wireup
{
    public class RedisQueueModule : ScopedModule<QueueConfiguration<EndPoint>>
    {
        public RedisQueueModule(QueueConfiguration<EndPoint> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
        }
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterOneInstanceForObjectKey<EndPoint, IRedisClient>((connection, context) => {
                var connectionManager = new RedisConnectionManager(context.Resolve<IRedisSocketAsyncEventArgsPool>(TypedParameter.From<EndPoint>(connection)));
                return new RedisClient(connectionManager); 
            });
            
           
            builder.RegisterWithScope<IRedisSocketAsyncEventArgsPool>((componentScope, parameters) =>
            {
                var endPoint = parameters.TypedAs<EndPoint>();
                return new RedisSocketAsyncEventArgsPool(10, endPoint);
            });

            
            builder.RegisterWithScope<IQueueFactory>((componentScope, parameters) =>
            {
                return new RedisQueueFactory(Configuration, componentScope.Resolve<ISerializer>(), componentScope.Resolve<RedisClientFactory>());
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<RedisClientFactory>(componentScope =>
            {
                 return endPoint => componentScope.Resolve<IRedisClient>(TypedParameter.From<EndPoint>(endPoint));
            }).InstancePerMatchingLifetimeScope(Scope);

           
        }
    }
}
