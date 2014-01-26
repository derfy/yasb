using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Yasb.Common.Messaging.Configuration;
using Yasb.MongoDb.Messaging;
using Autofac.Core;
using Yasb.MongoDb.Messaging.Configuration;

namespace Yasb.Wireup
{
    public class MongoDbConfigurator : AbstractConfigurator<MongoDbEndPointConfiguration, SubscriptionServiceConfiguration>
    {
        protected override IModule RegisterServiceBusModule(ServiceBusConfiguration<MongoDbEndPointConfiguration, SubscriptionServiceConfiguration> serviceBusConfiguration)
        {
            return new MongoDbServiceBusModule(serviceBusConfiguration);
        }

       
    }
}
