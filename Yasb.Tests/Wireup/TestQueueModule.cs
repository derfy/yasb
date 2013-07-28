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
        private Dictionary<string, Mock<IQueue<TestConnection>>> _queueFactory = new Dictionary<string, Mock<IQueue<TestConnection>>>();
        private string _localEndPoint;
        string _endPointValue0 = "192.168.127.128:6379:consumer";
        string _endPointValue1 = "192.168.127.128:6379:producer";
        string _endPointValue2 = "192.168.127.128:6379:myQueue";
        string _endPointValue3 = "192.168.127.128:6379:anotherQueue";
        public TestQueueFactory(QueueConfiguration<TestConnection> queueConfiguration) : base(queueConfiguration)
        {
            var endPoint0 = new Mock<QueueEndPoint<TestConnection>>();
            endPoint0.Setup(e => e.Value).Returns(_endPointValue0);
            var endPoint1 = new Mock<QueueEndPoint<TestConnection>>();
            endPoint1.Setup(e => e.Value).Returns(_endPointValue1);
            var endPoint2 = new Mock<QueueEndPoint<TestConnection>>();
            endPoint2.Setup(e => e.Value).Returns(_endPointValue2);
            var endPoint3 = new Mock<QueueEndPoint<TestConnection>>();
            endPoint3.Setup(e => e.Value).Returns(_endPointValue3);
            var repo = new MockRepository(MockBehavior.Default);
            _queueFactory[_endPointValue0] = repo.Create<IQueue<TestConnection>>();
            _queueFactory[_endPointValue0].Setup(q => q.LocalEndPoint).Returns(endPoint0.Object);
            _queueFactory[_endPointValue1] = repo.Create<IQueue<TestConnection>>();
            _queueFactory[_endPointValue1].Setup(q => q.LocalEndPoint).Returns(endPoint1.Object);
            _queueFactory[_endPointValue2] = repo.Create<IQueue<TestConnection>>();
            _queueFactory[_endPointValue3] = repo.Create<IQueue<TestConnection>>();
            _localEndPoint = _endPointValue0;
        }
        public Mock<IQueue<TestConnection>> GetMock(string endPoint)
        {
            return _queueFactory[endPoint];
        }
        
        public override IQueue<TestConnection> CreateQueue(TestConnection connection, string queueName)
        {
            return _queueFactory[string.Format("{0}:{1}", connection, queueName)].Object;
        }
    }
    public class TestQueueModule : ScopedModule<QueueConfiguration<TestConnection>>
    {
        private AbstractQueueFactory<TestConnection> _queueFactory;
        public TestQueueModule(AbstractQueueFactory<TestConnection> queueFactory, QueueConfiguration<TestConnection> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
            _queueFactory = queueFactory;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<AbstractQueueFactory<TestConnection>>((componentScope, parameters) =>
            {
                return _queueFactory;
            }).InstancePerMatchingLifetimeScope(Scope);
            
            
        }
    }

    public class TestServiceBusModule : ServiceBusModule<TestConnection>
    {
        Mock<ISubscriptionService<TestConnection>> _subscriptionService;
        public TestServiceBusModule(Mock<ISubscriptionService<TestConnection>> subscriptionService, ServiceBusConfiguration<TestConnection> configuration)
            : base(configuration)
        {
            _subscriptionService = subscriptionService;
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<ISubscriptionService<TestConnection>>((componentScope, parameters) =>
            {

                return _subscriptionService.Object;
            });
        }
    }
}
