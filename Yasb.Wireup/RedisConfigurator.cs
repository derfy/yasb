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
using Yasb.Common.Messaging.Connections;

namespace Yasb.Wireup
{


    public class RedisConfigurator : AbstractConfigurator<RedisConnection>
    {

        protected override void RegisterServiceBusModule(ContainerBuilder builder, ServiceBusConfiguration<RedisConnection> serviceBusConfiguration)
        {
            builder.RegisterModule(new CommonModule<ServiceBusConfiguration<RedisConnection>>(serviceBusConfiguration, "bus"));
            builder.RegisterModule(new RedisQueueModule(serviceBusConfiguration, "bus"));
            builder.RegisterModule(new RedisServiceBusModule(serviceBusConfiguration));
            
        }

        protected override void RegisterQueueModule(ContainerBuilder builder, QueueConfiguration<RedisConnection> queueConfiguration)
        {
            builder.RegisterModule(new CommonModule<QueueConfiguration<RedisConnection>>(queueConfiguration, "queue"));
            builder.RegisterModule(new RedisQueueModule(queueConfiguration,"queue"));
        }
    }
}
