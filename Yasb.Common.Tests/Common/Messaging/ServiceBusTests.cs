using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;
using Yasb.Common;
using Moq;
using System.Threading;
using Yasb.Common.Messaging.Configuration;
using Yasb.Redis.Messaging;
using Yasb.Tests.Common.Messaging.Configuration;
using Yasb.Tests.Common.Serialization;

namespace Yasb.Tests.Common.Messaging
{
    
    
   
    [TestClass]
    public class ServiceBusTests
    {
        private  Mock<IWorker> _messagesReceiver;
        private Mock<ITaskRunner> _workersManager;
        private  Mock<ISubscriptionService> _subscriptionService;
        private IServiceBus _sut;
        private Dictionary<IEndPoint, Mock<IQueue>> dict = new Dictionary<IEndPoint, Mock<IQueue>>();
        private ServiceBusConfiguration _configuration = new Mock<ServiceBusConfiguration>().Object;
        [TestInitialize]
        public void Setup()
        {
            var repo=new MockRepository(MockBehavior.Default);
            var endPoint0 = new TestEndPoint("localhost:80:myQueue");
            dict[endPoint0] = repo.Create<IQueue>();
            dict[endPoint0].SetupGet(q => q.LocalEndPoint).Returns(endPoint0);
            var endPoint1=new TestEndPoint("myTestAddress1:80:myQueue");
            dict[endPoint1] = repo.Create<IQueue>();
            dict[endPoint1].SetupGet(q => q.LocalEndPoint).Returns(endPoint1);
            var endPoint2 = new TestEndPoint("myTestAddress2:80:myQueue");
            dict[endPoint2] = repo.Create<IQueue>();
            dict[endPoint2].SetupGet(q => q.LocalEndPoint).Returns(endPoint2);
            var endPoint3 = new TestEndPoint("localhost:80:theirQueue");
            dict[endPoint3] = repo.Create<IQueue>();
            dict[endPoint3].SetupGet(q => q.LocalEndPoint).Returns(endPoint3);

            _configuration.WithLocalEndPoint<TestEndPointConfiguration>("localhost:80:myQueue")
                           .WithEndPoint<TestEndPointConfiguration>("localhost:80:theirQueue", e => e.WithName("foo"));
            _messagesReceiver = new Mock<IWorker>();
            _subscriptionService = new Mock<ISubscriptionService>();
            _workersManager=new Mock<ITaskRunner>();
            _sut = new ServiceBus(_configuration.NamedEndPoints, _messagesReceiver.Object, MapQueue, _subscriptionService.Object, _workersManager.Object);
        }
        [TestMethod]
        public void LocalMessageEndPointShouldBeCorrect()
        {
            Assert.AreEqual<IEndPoint>(_configuration.LocalEndPoint, _sut.LocalEndPoint);
        }
        [TestMethod]
        public void WhenSendingMessageSenderShouldBeInvokedCorrectly()
        {
             var message=new TestMessage();
             var endPoint = _configuration.NamedEndPoints.First(e=>e.Name=="foo");
             _sut.Send("foo", message);
             var testEnvelope = new TestEnvelope();
             dict[endPoint].Verify(s => s.WrapInEnvelope(message, _configuration.LocalEndPoint), Times.Once());
        }
        [TestMethod]
        public void ShouldBeAbleToSubscribe()
        {
            var message = new SubscriptionMessage() { TypeName = typeof(TestMessage).FullName, SubscriberEndPoint = _configuration.LocalEndPoint };
            var endPoint = _configuration.NamedEndPoints.First(e => e.Name == "foo");
            _sut.Subscribe<TestMessage>("foo");
            dict[endPoint].Verify(s => s.WrapInEnvelope(It.Is<SubscriptionMessage>(p => p.TypeName == typeof(TestMessage).FullName), _configuration.LocalEndPoint), Times.Once());
        }

        [TestMethod]
        public void ShouldBeAbleToPublish()
        {
            var message = new TestMessage();
            var endPoint1 = new TestEndPoint("myTestAddress1:80:myQueue");
            var endPoint2 = new TestEndPoint("myTestAddress2:80:myQueue");
            _subscriptionService.Setup(s => s.GetSubscriptionEndPoints(typeof(TestMessage).FullName)).Returns(new TestEndPoint[] { endPoint1, endPoint2 });

            var testEnvelope1 = new MessageEnvelope();
            var testEnvelope2 = new MessageEnvelope();
            dict[endPoint1].Setup(q => q.WrapInEnvelope(message, _configuration.LocalEndPoint)).Returns(testEnvelope1);
            dict[endPoint2].Setup(q => q.WrapInEnvelope(message, _configuration.LocalEndPoint)).Returns(testEnvelope2);
            _sut.Publish(message);
            dict[endPoint1].Verify(q => q.WrapInEnvelope(message, _configuration.LocalEndPoint), Times.Once());

            dict[endPoint2].Verify(q => q.WrapInEnvelope(message, _configuration.LocalEndPoint), Times.Once());
            
            dict[endPoint1].Verify(s => s.Push(testEnvelope1), Times.Once());
            dict[endPoint2].Verify(s => s.Push(testEnvelope2), Times.Once());
        }
        [TestMethod]
        public void ShouldBeAbleToRun()
        {
            var token = new CancellationToken();
            _workersManager.Setup(w => w.Run(It.IsAny<IWorker>())).Callback((IWorker worker) => 
            {
                worker.Execute(token);
            });
            _sut.Run();
            _messagesReceiver.Verify(r => r.Execute(token),Times.Once());
           
        }



        IQueue MapQueue(IEndPoint endPoint)
        {
            return dict[endPoint].Object;
        }
    }
}
