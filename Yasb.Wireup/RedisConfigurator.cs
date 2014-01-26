using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using System.Collections.Concurrent;
using System.Threading;
using System.Reflection;
using System.Linq.Expressions;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;
using Newtonsoft.Json;
using System.Net;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Configuration;
using Autofac.Core;

namespace Yasb.Wireup
{


    public class RedisConfigurator : AbstractConfigurator<RedisEndPointConfiguration, RedisSubscriptionServiceConfiguration> 
    {

        protected override IModule RegisterServiceBusModule(ServiceBusConfiguration<RedisEndPointConfiguration, RedisSubscriptionServiceConfiguration> serviceBusConfiguration)
        {
            return new RedisServiceBusModule(serviceBusConfiguration);
            
        }

    }
}
