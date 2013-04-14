using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Moq;
using Autofac;
using Yasb.Common.Tests;
namespace Yasb.Wireup
{
    public class TestModule: Autofac.Module
    {
        Mock<ISubscriptionService> _subscriptionService;
        private Dictionary<BusEndPoint, Mock<IQueue>> _dict = new Dictionary<BusEndPoint, Mock<IQueue>>();
        public TestModule(Dictionary<BusEndPoint, Mock<IQueue>> dict, Mock<ISubscriptionService> subscriptionService)
        {
            this._dict = dict;
            _subscriptionService = subscriptionService;
        }

        
        protected override void Load(Autofac.ContainerBuilder builder)
        {
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
