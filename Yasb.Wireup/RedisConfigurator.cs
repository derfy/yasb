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
using Yasb.Common.Messaging.EndPoints.Redis;
using Yasb.Common.Messaging.Configuration.Redis;
using Yasb.Common.Serialization.Json;

namespace Yasb.Wireup
{


    public class RedisConfigurator : AbstractConfigurator<RedisEndPoint, JsonNetSerializer<RedisEndPoint>> 
    {

        protected override void RegisterServiceBusModule(ContainerBuilder builder, ServiceBusConfiguration<RedisEndPoint, JsonNetSerializer<RedisEndPoint>> serviceBusConfiguration)
        {
            builder.RegisterModule(new RedisServiceBusModule(serviceBusConfiguration));
            
        }

        protected override void RegisterQueueModule(ContainerBuilder builder, ServiceBusConfiguration<RedisEndPoint, JsonNetSerializer<RedisEndPoint>> queueConfiguration)
        {
            builder.RegisterModule(new RedisQueueModule(queueConfiguration,"queue"));
        }
    }
}
