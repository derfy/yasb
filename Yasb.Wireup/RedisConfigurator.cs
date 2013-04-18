using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using System.Collections.Concurrent;
using System.Threading;
using System.Reflection;
using System.Linq.Expressions;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;
using Newtonsoft.Json;
using System.Net;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Redis.Messaging.Client;

namespace Yasb.Wireup
{
    

    public class RedisConfigurator : AbstractConfigurator<EndPoint>
    {

        protected override void RegisterServiceBusModule(ServiceBusConfiguration<EndPoint> serviceBusConfiguration)
        {
            Builder.RegisterModule(new RedisQueueModule(serviceBusConfiguration.EndPointConfiguration, "bus"));
            Builder.RegisterModule(new RedisServiceBusModule(serviceBusConfiguration));
        }

        protected override void RegisterQueueModule(EndPointConfiguration<EndPoint> endPointConfiguration)
        {
            Builder.RegisterModule(new RedisQueueModule(endPointConfiguration,"queue"));
        }
    }
}
