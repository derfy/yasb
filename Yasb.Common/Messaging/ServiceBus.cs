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
        private readonly Func<ISubscriptionService> _subscriptionServiceFactory;
        private readonly IEnumerable<BusEndPoint> _endPointsList;
        public ServiceBus(IEnumerable<BusEndPoint> endPointsList, IWorker messagesReceiver, IMessagesSender messagesSender, Func<ISubscriptionService> subscriptionServiceFactory, ITaskRunner taskRunner)
        {
            _messagesSender = messagesSender;
            _messagesReceiver = messagesReceiver;
            _taskRunner = taskRunner;
            _subscriptionServiceFactory = subscriptionServiceFactory;
            _endPointsList = endPointsList;
        }

        public virtual BusEndPoint LocalEndPoint { get { return _endPointsList.First(e=>e.Name=="local"); } }
        
        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            using (var subscriptionSevice = _subscriptionServiceFactory())
            {
                var subscriptionPoints = subscriptionSevice.GetSubscriptionEndPoints(message.GetType().FullName);
                foreach (var endPoint in subscriptionPoints)
                {
                    Send<TMessage>(endPoint, message);
                }
            }
            
        }
        public void Send<TMessage>(string endPointName, TMessage message) where TMessage : IMessage
        {
            var endPoint = _endPointsList.First(e => e.Name == endPointName);
            Send(endPoint, message);
        }
        public void Subscribe<TMessage>(string endPointName) where TMessage : IMessage
        {
            var endPoint = _endPointsList.First(e => e.Name == endPointName);
            Subscribe<TMessage>(endPoint);
        }
        public void Subscribe<TMessage>(BusEndPoint endPoint) where TMessage : IMessage
        {
            var subscriptionMessage = new SubscriptionMessage() { TypeName = typeof(TMessage).FullName, SubscriberEndPoint = LocalEndPoint };
            Send<SubscriptionMessage>(endPoint, subscriptionMessage);
        }
        public void Send<TMessage>(BusEndPoint endPoint, TMessage message) where TMessage : IMessage
        {
            var envelope = new MessageEnvelope(message, Guid.NewGuid(), LocalEndPoint, endPoint);
            _messagesSender.Send(endPoint, envelope);
        }
        
        public void Run()
        {
            _taskRunner.Run(_messagesReceiver);
        }
    }
}
