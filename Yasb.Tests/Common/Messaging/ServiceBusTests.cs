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
using Yasb.Tests.Common.Serialization;
using Yasb.Common.Tests.Configuration;
using Yasb.Wireup;
using Yasb.Common.Tests;
using System.Net;
using Yasb.Tests.Wireup;

namespace Yasb.Tests.Common.Messaging
{
    
    
   
    [TestClass]
    public class ServiceBusTests
    {
        private Mock<ISubscriptionService<TestConnection>> _subscriptionService;
        private IServiceBus<TestConnection> _sut;
        private TestQueueFactory _queueFactory; 
        [TestInitialize]
        public void Setup()
        {
           
            _subscriptionService = new Mock<ISubscriptionService<TestConnection>>();
            var configurator = new TestConfigurator();
            configurator.SubscriptionService = _subscriptionService;
            _sut = configurator.Bus(sb => sb.WithEndPointConfiguration(ec => ec.WithLocalEndPoint("vmEndPoint", "consumer")
                                                                               .WithEndPoint("vmEndPoint", "producer", "producer"))
                                            .ConfigureConnections<FluentTestConnectionConfigurer>(c => c.WithConnection("vmEndPoint", "192.168.127.128"))
                                            .WithMessageHandlersAssembly(typeof(ExampleMessage).Assembly));
            _queueFactory = configurator.QueueFactory;
           
        }
        [TestMethod]
        public void LocalMessageEndPointShouldBeCorrect()
        {
            Assert.AreEqual<string>("192.168.127.128:6379:consumer", _sut.LocalEndPoint.Value);
        }
        [TestMethod]
        public void WhenSendingMessageQueueShouldBeInvokedCorrectly()
        {
             var message=new TestMessage("foo");
             _sut.Send("producer", message);
            var endPoint0 = new Mock<QueueEndPoint<TestConnection>>();
            var connection = new TestConnection("192.168.127.128", 6379);
            endPoint0.Setup(e => e.Connection).Returns(connection);
            endPoint0.Setup(e => e.Name).Returns("producer");
            endPoint0.Setup(e => e.Value).Returns("192.168.127.128:6379:producer");
            var queue = _queueFactory.GetMock(endPoint0.Object.Value);
            queue.Verify(s => s.Push(message, "192.168.127.128:6379:consumer", It.IsAny<string>()), Times.Once());
      
        }
        [TestMethod]
        public void ShouldBeAbleToSubscribe()
        {
             _sut.Subscribe<TestMessage>("producer");
             _queueFactory.GetMock("192.168.127.128:6379:producer").Verify(s => s.Push(It.IsAny<SubscriptionMessage<TestConnection>>(), "192.168.127.128:6379:consumer", It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void ShouldBeAbleToPublish()
        {
            var endPoint1=new Mock<QueueEndPoint<TestConnection>>();
            var connection = new TestConnection("192.168.127.128", 6379);
            endPoint1.Setup(e => e.Connection).Returns(connection);
            endPoint1.Setup(e => e.Name).Returns("consumer");
             var endPoint2=new Mock<QueueEndPoint<TestConnection>>();
             endPoint2.Setup(e => e.Connection).Returns(connection);
             endPoint2.Setup(e => e.Name).Returns("producer");
            _subscriptionService.Setup(s => s.GetSubscriptions(typeof(TestMessage).AssemblyQualifiedName)).Returns(new SubscriptionInfo<TestConnection>[] { new SubscriptionInfo<TestConnection>(endPoint1.Object, ""), new SubscriptionInfo<TestConnection>(endPoint2.Object, "") });
          
            var message = new TestMessage("foo");
             _sut.Publish(message);
             _queueFactory.GetMock("192.168.127.128:6379:consumer").Verify(s => s.Push(message, "192.168.127.128:6379:consumer", It.IsAny<string>()), Times.Once());
             _queueFactory.GetMock("192.168.127.128:6379:producer").Verify(s => s.Push(message, "192.168.127.128:6379:consumer", It.IsAny<string>()), Times.Once());
             _queueFactory.GetMock("192.168.127.128:6379:myQueue").Verify(s => s.Push(It.IsAny<IMessage>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }
        [TestMethod]
        public void ShouldBeAbleToRun()
        {
            //var token = new CancellationToken();
            //_workersManager.Setup(w => w.Run(It.IsAny<IWorker>())).Callback((IWorker worker) => 
            //{
            //    worker.Execute(token);
            //});
            //_sut.Run();
            //_messagesReceiver.Verify(r => r.Execute(token),Times.Once());
           
        }


    }
}
