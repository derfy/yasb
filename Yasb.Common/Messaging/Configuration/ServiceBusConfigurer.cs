using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Messaging.Configuration
{

    public class ServiceBusConfigurer<TEndPointConfiguration, TSubscriptionServiceConfiguration> 
    {
        public ServiceBusConfigurer()
        {
            Built = new ServiceBusConfiguration<TEndPointConfiguration, TSubscriptionServiceConfiguration>();
        }
        public ServiceBusConfigurer<TEndPointConfiguration, TSubscriptionServiceConfiguration> EndPoints(Action<EndPointsConfigurer<TEndPointConfiguration>> endPointConfigurationBuilder)
        {
            var endPointsConfigurer = new EndPointsConfigurer<TEndPointConfiguration>(Built.EndPoints);
            endPointConfigurationBuilder(endPointsConfigurer);
            return this;
        }
        public ServiceBusConfigurer<TEndPointConfiguration, TSubscriptionServiceConfiguration> ConfigureSubscriptionService(Action<TSubscriptionServiceConfiguration> subscriptionServiceConfigurationBuilder)
        {
           subscriptionServiceConfigurationBuilder(Built.SubscriptionServiceConfiguration);
            return this;
        }
        public ServiceBusConfiguration<TEndPointConfiguration, TSubscriptionServiceConfiguration> Built { get; private set; }
       
       
    }
}
