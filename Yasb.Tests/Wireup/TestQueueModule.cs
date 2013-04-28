using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Tests.Configuration;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Wireup;
using Moq;

namespace Yasb.Tests.Wireup
{
    public class TestQueueFactory : AbstractQueueFactory<TestConnection>
    {
        private Dictionary<string, Mock<IQueue>> _queueFactory = new Dictionary<string, Mock<IQueue>>();
        private string _localEndPoint;
        string _endPointValue0 = "192.168.127.128:6379:consumer";
        string _endPointValue1 = "192.168.127.128:6379:producer";
        string _endPointValue2 = "192.168.127.128:6379:myQueue";
        string _endPointValue3 = "192.168.127.128:6379:anotherQueue";
        public TestQueueFactory(QueueConfiguration<TestConnection> queueConfiguration) : base(queueConfiguration)
        {
            var repo = new MockRepository(MockBehavior.Default);
            _queueFactory[_endPointValue0] = repo.Create<IQueue>();
            _queueFactory[_endPointValue0].Setup(q => q.LocalEndPoint).Returns(_endPointValue0);
            _queueFactory[_endPointValue1] = repo.Create<IQueue>();
            _queueFactory[_endPointValue1].Setup(q => q.LocalEndPoint).Returns(_endPointValue1);
            _queueFactory[_endPointValue2] = repo.Create<IQueue>();
            _queueFactory[_endPointValue3] = repo.Create<IQueue>();
            _localEndPoint = _endPointValue0;
        }
        public Mock<IQueue> GetMock(string endPoint){
            return _queueFactory[endPoint];
        }
        public override IQueue CreateFromEndPointValue(string endPointValue)
        {
            return _queueFactory[endPointValue].Object;
        }

        protected override IQueue CreateQueue(TestConnection connection, string queueName)
        {
            return _queueFactory[string.Format("{0}:{1}", connection, queueName)].Object;
        }
    }
    public class TestQueueModule : CommonModule<QueueConfiguration<TestConnection>>
    {
        private IQueueFactory _queueFactory;
        public TestQueueModule(IQueueFactory queueFactory,QueueConfiguration<TestConnection> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
            _queueFactory = queueFactory;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<IQueueFactory>((componentScope, parameters) =>
            {
                return _queueFactory;
            }).InstancePerMatchingLifetimeScope(Scope);
            
            
        }
    }

    public class TestServiceBusModule : ServiceBusModule<TestConnection>
    {
        Mock<ISubscriptionService> _subscriptionService;
        public TestServiceBusModule(Mock<ISubscriptionService> subscriptionService, ServiceBusConfiguration<TestConnection> configuration):base(configuration)
        {
            _subscriptionService = subscriptionService;
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<ISubscriptionService>((componentScope, parameters) =>
            {

                return _subscriptionService.Object;
            });
        }
    }
}
