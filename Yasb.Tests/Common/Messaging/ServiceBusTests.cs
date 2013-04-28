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
        private  Mock<ISubscriptionService> _subscriptionService;
        private IServiceBus _sut;
        private TestQueueFactory _queueFactory; 
        [TestInitialize]
        public void Setup()
        {
           
            _subscriptionService = new Mock<ISubscriptionService>();
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
            Assert.AreEqual<string>("192.168.127.128:6379:consumer", _sut.LocalEndPoint);
        }
        [TestMethod]
        public void WhenSendingMessageSenderShouldBeInvokedCorrectly()
        {
             var message=new TestMessage();
             _sut.Send("producer", message);
             _queueFactory.GetMock("192.168.127.128:6379:producer").Verify(s => s.Push(It.Is<MessageEnvelope>(e => e.Message == message && e.To.Equals("192.168.127.128:6379:producer"))), Times.Once());
      
        }
        [TestMethod]
        public void ShouldBeAbleToSubscribe()
        {
             _sut.Subscribe<TestMessage>("producer");
             _queueFactory.GetMock("192.168.127.128:6379:producer").Verify(s => s.Push(It.Is<MessageEnvelope>(e => e.Message.GetType() == typeof(SubscriptionMessage) && e.To.Equals("192.168.127.128:6379:producer"))), Times.Once());
        }

        [TestMethod]
        public void ShouldBeAbleToPublish()
        {

            _subscriptionService.Setup(s => s.GetSubscriptionEndPoints(typeof(TestMessage).FullName)).Returns(new string[] { "192.168.127.128:6379:consumer", "192.168.127.128:6379:producer" });
          
            var message = new TestMessage();
             _sut.Publish(message);
             _queueFactory.GetMock("192.168.127.128:6379:consumer").Verify(s => s.Push(It.Is<MessageEnvelope>(e => e.Message == message && e.From.Equals("192.168.127.128:6379:consumer") && e.To.Equals("192.168.127.128:6379:consumer"))), Times.Once());
             _queueFactory.GetMock("192.168.127.128:6379:producer").Verify(s => s.Push(It.Is<MessageEnvelope>(e => e.Message == message && e.From.Equals("192.168.127.128:6379:consumer") && e.To.Equals("192.168.127.128:6379:producer"))), Times.Once());
             _queueFactory.GetMock("192.168.127.128:6379:myQueue").Verify(s => s.Push(It.IsAny<MessageEnvelope>()), Times.Never());
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
