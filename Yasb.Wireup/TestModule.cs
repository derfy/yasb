using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Moq;
using Autofac;
using Yasb.Common.Tests;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Tests.Configuration;
namespace Yasb.Wireup
{
    public class TestModule : ServiceBusModule<ServiceBusConfiguration<TestConnectionConfiguration>, TestConnectionConfiguration>
    {
        Mock<ISubscriptionService> _subscriptionService;
        private Dictionary<BusEndPoint, Mock<IQueue>> _dict = new Dictionary<BusEndPoint, Mock<IQueue>>();
        public TestModule(ServiceBusConfiguration<TestConnectionConfiguration> configuration,Dictionary<BusEndPoint, Mock<IQueue>> dict, Mock<ISubscriptionService> subscriptionService):base(configuration)
        {
            this._dict = dict;
            _subscriptionService = subscriptionService;
        }

        
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<IQueue>((componentScope, parameters) =>
            {
                var endPoint = parameters.Named<BusEndPoint>("endPoint");
                return _dict[endPoint].Object;
            });
            builder.RegisterWithScope<ISubscriptionService>((componentScope, parameters) =>
            {
                
                return _subscriptionService.Object;
            });
        }
    }
}
