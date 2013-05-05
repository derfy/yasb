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
using Newtonsoft.Json;

namespace Yasb.Wireup
{

    public class RedisServiceBusModule : ServiceBusModule<EndPoint>
    {
        public RedisServiceBusModule(ServiceBusConfiguration<EndPoint> serviceBusConfiguration)
            : base(serviceBusConfiguration)
        {
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<ISubscriptionService>((componentScope, parameters) =>
            {
                var localEndPointInfo = Configuration.EndPointConfiguration.GetEndPointInfoByName("local");
                var connection = Configuration.ConnectionConfiguration.GetConnectionByName(localEndPointInfo.ConnectionName);
                return new SubscriptionService(componentScope.Resolve<IRedisClient>(TypedParameter.From<EndPoint>(connection)));
            });
        }
    }
   
}
