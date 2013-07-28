﻿using System;
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
        public void WhenSendingMessageSenderShouldBeInvokedCorrectly()
        {
             var message=new TestMessage("foo");
             _sut.Send("producer", message);
             var endPoint0 = new Mock<QueueEndPoint<TestConnection>>();
             _queueFactory.GetMock("192.168.127.128:6379:producer").Verify(s => s.CreateMessageEnvelope(message, It.Is<QueueEndPoint<TestConnection>>(m => m.Value == "192.168.127.128:6379:consumer"),It.IsAny<string>()), Times.Once());
      
        }
        [TestMethod]
        public void ShouldBeAbleToSubscribe()
        {
             _sut.Subscribe<TestMessage>("producer");
             _queueFactory.GetMock("192.168.127.128:6379:producer").Verify(s => s.CreateMessageEnvelope(It.IsAny<SubscriptionMessage<TestConnection>>(), It.Is<QueueEndPoint<TestConnection>>(m => m.Value == "192.168.127.128:6379:consumer"), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void ShouldBeAbleToPublish()
        {

         //   _subscriptionService.Setup(s => s.GetSubscriptions(typeof(TestMessage).FullName)).Returns(new string[] { "192.168.127.128:6379:consumer", "192.168.127.128:6379:producer" });
          
            var message = new TestMessage("foo");
             _sut.Publish(message);
             _queueFactory.GetMock("192.168.127.128:6379:consumer").Verify(s => s.CreateMessageEnvelope(message, It.Is<QueueEndPoint<TestConnection>>(m => m.Value == "192.168.127.128:6379:consumer"), It.IsAny<string>()), Times.Once());
             _queueFactory.GetMock("192.168.127.128:6379:producer").Verify(s => s.CreateMessageEnvelope(message, It.Is<QueueEndPoint<TestConnection>>(m => m.Value == "192.168.127.128:6379:consumer"), It.IsAny<string>()), Times.Once());
             _queueFactory.GetMock("192.168.127.128:6379:myQueue").Verify(s => s.CreateMessageEnvelope(It.IsAny<IMessage>(), It.IsAny<QueueEndPoint<TestConnection>>(), It.IsAny<string>()), Times.Never());
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
