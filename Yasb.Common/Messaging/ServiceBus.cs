using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Messaging
{
    public class ServiceBus<TEndPoint> : IServiceBus<TEndPoint> 
    {
        private readonly IQueueFactory<TEndPoint> _queueFactory;
        private readonly IWorkerPool _messageReveiverWorkerPool;
        private readonly ISubscriptionService<TEndPoint> _subscriptionService;
        private Func<string, TEndPoint> _endPointsFactory;
        public ServiceBus(Func<string, TEndPoint> endPointsFactory, IQueueFactory<TEndPoint> queueFactory, ISubscriptionService<TEndPoint> subscriptionService, MessagesReceiver<TEndPoint> messageReceiver)
        {
            _endPointsFactory = endPointsFactory;
            _queueFactory = queueFactory;
            _subscriptionService = subscriptionService;
            _messageReveiverWorkerPool = new WorkerPool(messageReceiver);
        }

        public virtual TEndPoint LocalEndPoint { get { return _endPointsFactory("local"); } }

        public void Send(string endPointName, IMessage message)
        {
            var endPoint = _endPointsFactory(endPointName);
            var queue = _queueFactory.CreateQueue(endPoint);
            queue.Push(message);
        }

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            var subscriptions = _subscriptionService.GetSubscriptionEndPoints();
            foreach (var subscriptionEndPoint in subscriptions)
            {
                var queue = _queueFactory.CreateQueue(subscriptionEndPoint);
                queue.Push(message);
            }
            
        }

        public void Subscribe(string topicEndPointName)
        {
            var topicEndPoint = _endPointsFactory(topicEndPointName);
            _subscriptionService.SubscribeTo(topicEndPoint);
        }
       
        public void Run()
        {
            _messageReveiverWorkerPool.Run();
        }

       

        public void Dispose()
        {
        }
    }
}
