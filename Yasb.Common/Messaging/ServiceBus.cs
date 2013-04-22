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
        private readonly QueueFactory _queueFactory;
        private readonly ITaskRunner _taskRunner;
        private readonly ISubscriptionService _subscriptionService;
        private readonly List<BusEndPoint> _namedEndPointsList = new List<BusEndPoint>();
        public ServiceBus(BusEndPoint[] namedEndPoints, IWorker messagesReceiver, QueueFactory queueFactory, ISubscriptionService subscriptionService, ITaskRunner taskRunner)
        {
            _queueFactory = queueFactory;
            _messagesReceiver = messagesReceiver;
            _taskRunner = taskRunner;
            _subscriptionService = subscriptionService;
            _namedEndPointsList.AddRange(namedEndPoints);
        }

        public virtual BusEndPoint LocalEndPoint { get { return GetEndPointByName("local"); } }
        
        public void Publish(IMessage message) 
        {
            var subscriptionEndPoints = _subscriptionService.GetSubscriptionEndPoints(message.GetType().FullName);
            foreach (var endPoint in subscriptionEndPoints)
            {
                Send(endPoint, message);
            }
            
        }
        public void Send(string endPointName, IMessage message)
        {
            var endPoint = GetEndPointByName(endPointName);
            Send(endPoint, message);
        }
        public void Subscribe<TMessage>(string endPointName) where TMessage : IMessage
        {
            var endPoint = GetEndPointByName(endPointName);
            Subscribe<TMessage>(endPoint);
        }


        public void Subscribe<TMessage>(BusEndPoint endPoint) where TMessage : IMessage
        {
            var subscriptionMessage = new SubscriptionMessage() { TypeName = typeof(TMessage).FullName, SubscriberEndPoint = LocalEndPoint };
            Send(endPoint, subscriptionMessage);
        }

        public void Send(BusEndPoint endPoint, IMessage message)
        {
            var queue = _queueFactory(endPoint);
            var envelope = new MessageEnvelope(message, LocalEndPoint, endPoint);
            queue.Push(envelope);
        }
        
        public void Run()
        {
            _taskRunner.Run(_messagesReceiver);
        }

        private BusEndPoint GetEndPointByName(string endPointName)
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
