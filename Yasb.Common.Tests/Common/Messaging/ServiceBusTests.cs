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

namespace Yasb.Tests.Common.Messaging
{
    public class FooMessage : IMessage
    { }
    [TestClass]
    public class ServiceBusTests
    {
        private  Mock<IWorker> _messagesReceiver;
        private  Mock<IMessagesSender> _messagesSender;
        private Mock<ITaskRunner> _workersManager;
        private  Mock<ISubscriptionService> _subscriptionService;
        private IServiceBus _sut;
        private ServiceBusConfiguration _configuration = new ServiceBusConfiguration();
        [TestInitialize]
        public void Setup()
        {
            _configuration.WithLocalEndPoint(e=>e.WithAddressInfo("foo",80))
                           .WithEndPoint("foo",e=>e.WithAddressInfo("foo",80).WithInputQueue("queue"));
            _messagesReceiver = new Mock<IWorker>();
            _messagesSender = new Mock<IMessagesSender>();
            _subscriptionService = new Mock<ISubscriptionService>();
            _workersManager=new Mock<ITaskRunner>();
            _sut = new ServiceBus(_configuration, _messagesReceiver.Object, _messagesSender.Object, _subscriptionService.Object, _workersManager.Object);
        }
        [TestMethod]
        public void LocalMessageEndPointShouldBeCorrect()
        {
            Assert.AreEqual<BusEndPoint>(_configuration.LocalEndPoint, _sut.LocalEndPoint);
        }
        [TestMethod]
        public void WhenSendingMessageSenderShouldBeInvokedCorrectly()
        {
             var message=new FooMessage();
             var endPoint = _configuration.GetEndPointByName("foo");
             _sut.Send<FooMessage>("foo", message);
            _messagesSender.Verify(s=>s.Send(endPoint,It.Is<MessageEnvelope>(env=>env.Message==message && env.To==endPoint && env.From==_sut.LocalEndPoint)),Times.Once());
        }
        [TestMethod]
        public void ShouldBeAbleToSubscribe()
        {
            var message = new SubscriptionMessage() { TypeName = typeof(FooMessage).FullName, SubscriberEndPoint = _configuration.LocalEndPoint };
            var endPoint = _configuration.GetEndPointByName("foo");
            _sut.Subscribe<FooMessage>("foo");
            _messagesSender.Verify(s => s.Send(endPoint, It.Is<MessageEnvelope>(env => ((SubscriptionMessage)env.Message).TypeName == typeof(FooMessage).FullName && ((SubscriptionMessage)env.Message).SubscriberEndPoint == _configuration.LocalEndPoint && env.To == endPoint && env.From == _sut.LocalEndPoint)), Times.Once());
        }

        [TestMethod]
        public void ShouldBeAbleToPublish()
        {
            var message = new FooMessage();
            var endPoint1=BusEndPoint.Parse("foo:80:endPoint1");
            var endPoint2 = BusEndPoint.Parse("foo:80:endPoint2");
            _subscriptionService.Setup(s => s.GetSubscriptionEndPoints(typeof(FooMessage).FullName)).Returns(new BusEndPoint[] { endPoint1,endPoint2 });
            _sut.Publish<FooMessage>(message);
            _messagesSender.Verify(s => s.Send(endPoint1, It.Is<MessageEnvelope>(env => env.Message == message  && env.To == endPoint1 && env.From == _sut.LocalEndPoint)), Times.Once());
            _messagesSender.Verify(s => s.Send(endPoint2, It.Is<MessageEnvelope>(env => env.Message == message && env.To == endPoint2 && env.From == _sut.LocalEndPoint)), Times.Once());
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
    }
}
