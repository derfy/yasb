using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Messaging
{
    public class ServiceBus : IServiceBus 
    {
        private readonly IWorker _messagesReceiver;
        private readonly IMessagesSender _messagesSender;
        private readonly ITaskRunner _taskRunner;
        private readonly ISubscriptionService _subscriptionService;
        public ServiceBus(BusEndPoint localEndPoint, IWorker messagesReceiver, IMessagesSender messagesSender, ISubscriptionService subscriptionService, ITaskRunner taskRunner)
        {
            _messagesSender = messagesSender;
            _messagesReceiver = messagesReceiver;
            _taskRunner = taskRunner;
            _subscriptionService = subscriptionService;
            LocalEndPoint = localEndPoint;
        }

        public virtual BusEndPoint LocalEndPoint { get; private set; }


        public void Send<TMessage>(BusEndPoint endPoint, TMessage message) where TMessage : IMessage
        {
            var envelope = new MessageEnvelope(message, Guid.NewGuid(),LocalEndPoint,endPoint);
            _messagesSender.Send(endPoint,envelope);
        }
        public void Subscribe<TMessage>(BusEndPoint endPoint) where TMessage : IMessage
        {
            var subscriptionMessage = new SubscriptionMessage() { TypeName = typeof(TMessage).FullName, SubscriberEndPoint=LocalEndPoint };
            Send<SubscriptionMessage>(endPoint, subscriptionMessage);
        }
        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            var subscriptionPoints = new BusEndPoint[] { BusEndPoint.Parse("192.168.127.128:6379:redis_consumer") };
           // var subscriptionPoints = _subscriptionService.GetSubscriptionEndPoints(message.GetType().FullName);
            foreach (var endPoint in subscriptionPoints)
            {
                Send<TMessage>(endPoint, message);
            }
        }

        public void Run()
        {
            _taskRunner.Run(_messagesReceiver);
        }
    }
}
