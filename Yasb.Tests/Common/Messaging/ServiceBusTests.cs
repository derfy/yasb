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
        private Mock<ISubscriptionService<TestEndPointConfiguration>> _subscriptionService;
        private IServiceBus<TestEndPointConfiguration> _sut;
        private Mock<IQueueFactory<TestEndPointConfiguration>> _queueFactory; 
        [TestInitialize]
        public void Setup()
        {

            _subscriptionService = new Mock<ISubscriptionService<TestEndPointConfiguration>>();
            //_endPointFactory = new Mock<IEndPointFactory<ITestEndPoint>>();
            _queueFactory = new Mock<IQueueFactory<TestEndPointConfiguration>>();
           var configurator = new TestConfigurator(_queueFactory);
            configurator.SubscriptionService = _subscriptionService;
            _sut = configurator.Bus(sb => sb.EndPoints(ec =>
                   ec.ReceivesOn(e => e.WithHostName("192.168.127.128").WithQueueName("consumer"))
                     .SubscribesTo("producer@vmEndPoint", e => e.WithHostName("vmEndPoint").WithQueueName("producer"))));
           
        }
        [TestMethod]
        public void LocalMessageEndPointShouldBeCorrect()
        {

            Assert.AreEqual<string>("192.168.127.128:6379:consumer", _sut.LocalEndPoint.Built.Value);
        }
        [TestMethod]
        public void WhenSendingMessageQueueShouldBeInvokedCorrectly()
        {

            var queue = new Mock<IQueue<TestEndPointConfiguration>>();
            _queueFactory.Setup(f => f.CreateQueue(It.Is<TestEndPointConfiguration>(e => e.Built.Host == "vmEndPoint" && e.Built.QueueName == "producer"))).Returns(queue.Object);
         
            var message = new TestMessage("foo");
            _sut.Send("producer@vmEndPoint", message);
            
            queue.Verify(s => s.Push(message), Times.Once());
      
        }
        [TestMethod]
        public void ShouldBeAbleToSubscribe()
        {
            var queue = new Mock<IQueue<TestEndPointConfiguration>>();
            _queueFactory.Setup(f => f.CreateQueue(It.Is<TestEndPointConfiguration>(e => e.Built.Host == "vmEndPoint" && e.Built.QueueName == "producer"))).Returns(queue.Object);
            _sut.Subscribe("producer@vmEndPoint");
            var envelope = new Mock<MessageEnvelope>();
            //queue.Verify(s => s.Push(It.IsAny<SubscriptionMessage>()), Times.Once());
        }

        [TestMethod]
        public void ShouldBeAbleToPublish()
        {
            var fooEndPoint = new TestEndPointConfiguration("192.168.127.128", "foo");
            var barEndPoint = new TestEndPointConfiguration("192.168.127.128", "bar");
            var queueFoo = new Mock<IQueue<TestEndPointConfiguration>>();
            var queueBar = new Mock<IQueue<TestEndPointConfiguration>>();
            _queueFactory.Setup(f => f.CreateQueue(fooEndPoint)).Returns(queueFoo.Object);
            _queueFactory.Setup(f => f.CreateQueue(barEndPoint)).Returns(queueBar.Object);
             var consumerEndPoint = new TestEndPoint("192.168.127.128", "consumer");
             _subscriptionService.Setup(s => s.GetSubscriptionEndPoints()).Returns(new TestEndPointConfiguration[] { fooEndPoint, barEndPoint });
          
            var message = new TestMessage("foo");
             _sut.Publish(message);
            queueFoo.Verify(s => s.Push(message), Times.Once());
             queueBar.Verify(s => s.Push(message), Times.Once());
             //_queueFactory.GetMock("192.168.127.128:6379:myQueue").Verify(s => s.Push(It.IsAny<IMessage>(), It.IsAny<string>()), Times.Never());
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
