using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Messaging
{
    public class ServiceBus<TEndPoint> : IServiceBus<TEndPoint>, IDisposable where TEndPoint : IEndPoint
    {
        private readonly IWorker _messagesReceiver;
        private readonly IMessagesSender<TEndPoint> _messagesSender;
        private readonly ITaskRunner _taskRunner;
        private readonly ISubscriptionService _subscriptionService;
        private readonly List<TEndPoint> _namedEndPointsList = new List<TEndPoint>();
        public ServiceBus(TEndPoint[] namedEndPoints, IWorker messagesReceiver, IMessagesSender<TEndPoint> messagesSender, ISubscriptionService subscriptionService, ITaskRunner taskRunner)
        {
            _messagesSender = messagesSender;
            _messagesReceiver = messagesReceiver;
            _taskRunner = taskRunner;
            _subscriptionService = subscriptionService;
            _namedEndPointsList.AddRange(namedEndPoints);
        }

        public virtual TEndPoint LocalEndPoint { get { return GetEndPointByName("local"); } }
        
        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            var subscriptionPoints = _subscriptionService.GetSubscriptionEndPoints(message.GetType().FullName);
            foreach (var endPoint in subscriptionPoints.OfType<TEndPoint>())
            {
                Send<TMessage>(endPoint, message);
            }
            
        }
        public void Send<TMessage>(string endPointName, TMessage message) where TMessage : IMessage
        {
            var endPoint = GetEndPointByName(endPointName);
            Send(endPoint, message);
        }
        public void Subscribe<TMessage>(string endPointName) where TMessage : IMessage
        {
            var endPoint = GetEndPointByName(endPointName);
            Subscribe<TMessage>(endPoint);
        }


        public void Subscribe<TMessage>(TEndPoint endPoint) where TMessage : IMessage
        {
            var subscriptionMessage = new SubscriptionMessage<TEndPoint>() { TypeName = typeof(TMessage).FullName, SubscriberEndPoint = LocalEndPoint };
            Send<SubscriptionMessage<TEndPoint>>(endPoint, subscriptionMessage);
        }
        public void Send<TMessage>(TEndPoint endPoint, TMessage message) where TMessage : IMessage
        {
            var envelope = new MessageEnvelope(message, Guid.NewGuid(), LocalEndPoint, endPoint);
            _messagesSender.Send(endPoint, envelope);
        }
        
        public void Run()
        {
            _taskRunner.Run(_messagesReceiver);
        }

        private TEndPoint GetEndPointByName(string endPointName)
        {
            var endPoint = _namedEndPointsList.FirstOrDefault(e => e.Name == endPointName);
            if (endPoint == null)
                throw new ApplicationException(string.Format("No Endpoint was registered with name {0}", endPointName));
            return endPoint;
        }

        public void Dispose()
        {
        }
    }
}
