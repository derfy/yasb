using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Msmq.Messaging.Configuration;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Wireup
{
    public class MsmqServiceBusModule : ServiceBusModule<ServiceBusConfiguration<MsmqConnectionConfiguration>, MsmqConnectionConfiguration> 
    {
        public MsmqServiceBusModule(ServiceBusConfiguration<MsmqConnectionConfiguration> serviceBusConfiguration):base(serviceBusConfiguration)
        {

        }
    }
}
