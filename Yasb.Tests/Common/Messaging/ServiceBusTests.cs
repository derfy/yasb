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

namespace Yasb.Tests.Common.Messaging
{
    
    
   
    [TestClass]
    public class ServiceBusTests
    {
        private  Mock<ISubscriptionService> _subscriptionService;
        private IServiceBus _sut;
        private Dictionary<BusEndPoint, Mock<IQueue>> _queueFactory = new Dictionary<BusEndPoint, Mock<IQueue>>();
        private BusEndPoint _localEndPoint;
        BusEndPoint _endPoint0= new BusEndPoint("vmEndPoint:consumer");
        BusEndPoint _endPoint1 = new BusEndPoint("vmEndPoint:producer");
        BusEndPoint _endPoint2 = new BusEndPoint("myConnection:myQueue");
        BusEndPoint _endPoint3 = new BusEndPoint("myConnection:anotherQueue");
        [TestInitialize]
        public void Setup()
        {
            

            var repo = new MockRepository(MockBehavior.Default);
            _queueFactory[_endPoint0] = repo.Create<IQueue>();
            _queueFactory[_endPoint1] = repo.Create<IQueue>();
            _queueFactory[_endPoint2] = repo.Create<IQueue>();
            _queueFactory[_endPoint3] = repo.Create<IQueue>();
            _localEndPoint = _endPoint0;
            _subscriptionService = new Mock<ISubscriptionService>();
            var configurator = new TestConfigurator(_queueFactory, _subscriptionService);
            _sut = configurator.Bus(sb => sb.WithEndPointConfiguration(ec => ec.WithLocalEndPoint("vmEndPoint", "consumer")
                 .WithEndPoint("vmEndPoint", "producer", "producer")).WithMessageHandlersAssembly(typeof(ExampleMessage).Assembly)
                 .ConfigureConnections<FluentTestConnectionConfigurer>(c => c.WithConnection("vmEndPoint", "192.168.127.128")));
        
        }
        [TestMethod]
        public void LocalMessageEndPointShouldBeCorrect()
        {
            Assert.AreEqual<BusEndPoint>(_localEndPoint, _sut.LocalEndPoint);
        }
        [TestMethod]
        public void WhenSendingMessageSenderShouldBeInvokedCorrectly()
        {
             var message=new TestMessage();
             _sut.Send("producer", message);
             _queueFactory[_endPoint1].Verify(s => s.Push(It.Is<MessageEnvelope>(e => e.Message == message && e.To.Equals(_endPoint1))), Times.Once());
      
        }
        [TestMethod]
        public void ShouldBeAbleToSubscribe()
        {
            var message = new SubscriptionMessage() { TypeName = typeof(TestMessage).FullName, SubscriberEndPoint = _localEndPoint };
            _sut.Subscribe<TestMessage>("producer");
            _queueFactory[_endPoint1].Verify(s => s.Push(It.Is<MessageEnvelope>(e => e.Message.GetType() == typeof(SubscriptionMessage) && e.To.Equals(_endPoint1))), Times.Once());
        }

        [TestMethod]
        public void ShouldBeAbleToPublish()
        {
         
            _subscriptionService.Setup(s => s.GetSubscriptionEndPoints(typeof(TestMessage).FullName)).Returns(new BusEndPoint[] { _endPoint0, _endPoint1 });
          
            var message = new TestMessage();
             _sut.Publish(message);
             _queueFactory[_endPoint0].Verify(s => s.Push(It.Is<MessageEnvelope>(e=>e.Message==message && e.From.Equals(_localEndPoint) &&e.To.Equals(_endPoint0))), Times.Once());
             _queueFactory[_endPoint1].Verify(s => s.Push(It.Is<MessageEnvelope>(e => e.Message == message && e.From.Equals(_localEndPoint) && e.To.Equals(_endPoint1))), Times.Once());
             _queueFactory[_endPoint2].Verify(s => s.Push(It.IsAny<MessageEnvelope>()), Times.Never());
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
