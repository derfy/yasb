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
        private readonly IServiceBusConfiguration _configuration;
        public ServiceBus(IServiceBusConfiguration configuration, IWorker messagesReceiver, IMessagesSender messagesSender, ISubscriptionService subscriptionService, ITaskRunner taskRunner)
        {
            _messagesSender = messagesSender;
            _messagesReceiver = messagesReceiver;
            _taskRunner = taskRunner;
            _subscriptionService = subscriptionService;
            _configuration = configuration;
        }

        public virtual BusEndPoint LocalEndPoint { get { return _configuration.GetEndPointByName("local"); } }
        
        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            var subscriptionPoints = _subscriptionService.GetSubscriptionEndPoints(message.GetType().FullName);
            foreach (var endPoint in subscriptionPoints)
            {
                Send<TMessage>(endPoint, message);
            }
            
        }
        public void Send<TMessage>(string endPointName, TMessage message) where TMessage : IMessage
        {
            var endPoint = _configuration.GetEndPointByName(endPointName);
            Send(endPoint, message);
        }
        public void Subscribe<TMessage>(string endPointName) where TMessage : IMessage
        {
            var endPoint = _configuration.GetEndPointByName(endPointName);
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
