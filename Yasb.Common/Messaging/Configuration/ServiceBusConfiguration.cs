using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Messaging.Configuration
{
    public class ServiceBusConfiguration<TEndPointConfiguration, TSubscriptionServiceConfiguration>
    {
        public ServiceBusConfiguration()
        {
            EndPoints = new EndPointsConfiguration<TEndPointConfiguration>();
            SubscriptionServiceConfiguration = Activator.CreateInstance<TSubscriptionServiceConfiguration>();
        }

        public EndPointsConfiguration<TEndPointConfiguration> EndPoints { get; private set; }

        public TSubscriptionServiceConfiguration SubscriptionServiceConfiguration { get; private set; }
    }
}
