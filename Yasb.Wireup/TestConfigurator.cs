using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Common.Tests.Configuration;
using Moq;

namespace Yasb.Wireup
{
   

    public class TestConfigurator :  AbstractConfigurator<TestConnectionConfiguration>
    {
        Mock<ISubscriptionService> _subscriptionService;
        private Mock<IQueue> _queue;
        Dictionary<BusEndPoint, Mock<IQueue>> _dict;
        public TestConfigurator(Dictionary<BusEndPoint, Mock<IQueue>> dict, Mock<ISubscriptionService> subscriptionService)
        {
            _subscriptionService = subscriptionService;
            _dict = dict;
        }
        protected override void RegisterServiceBusModule(ServiceBusConfiguration<TestConnectionConfiguration> serviceBusConfiguration)
        {
            Builder.RegisterModule(new TestModule(serviceBusConfiguration,_dict, _subscriptionService));
        }

        protected override void RegisterQueueModule(EndPointConfiguration<TestConnectionConfiguration> queueConfiguration)
        {
            throw new NotImplementedException();
        }
       



      
       
    }
}
