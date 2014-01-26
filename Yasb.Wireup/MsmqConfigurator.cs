using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Msmq.Messaging;
using System.Messaging;
using Yasb.Msmq.Messaging.Configuration;
using Autofac.Core;

namespace Yasb.Wireup
{

    public class MsmqConfigurator : AbstractConfigurator<MsmqEndPointConfiguration, MsmqSubscriptionServiceConfiguration>
    {


        protected override IModule RegisterServiceBusModule(ServiceBusConfiguration<MsmqEndPointConfiguration, MsmqSubscriptionServiceConfiguration> serviceBusConfiguration)
        {
            return new MsmqServiceBusModule(serviceBusConfiguration);
        }
       
       
       
    }
}
