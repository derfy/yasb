using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Messaging
{
    public class ServiceBus<TEndPointConfiguration> : IServiceBus<TEndPointConfiguration> 
    {
        private readonly IQueueFactory<TEndPointConfiguration> _queueFactory;
        private readonly IWorkerPool _messageReveiverWorkerPool;
        private readonly ISubscriptionService<TEndPointConfiguration> _subscriptionService;
        private EndPointsConfiguration<TEndPointConfiguration> _endPointsRepository;
        public ServiceBus(EndPointsConfiguration<TEndPointConfiguration> endPointsRepository, IQueueFactory<TEndPointConfiguration> queueFactory, ISubscriptionService<TEndPointConfiguration> subscriptionService, MessagesReceiver<TEndPointConfiguration> messageReceiver)
        {
            _endPointsRepository = endPointsRepository;
            _queueFactory = queueFactory;
            _subscriptionService = subscriptionService;
            _messageReveiverWorkerPool = new WorkerPool(messageReceiver);
        }

        public virtual TEndPointConfiguration LocalEndPoint { get { return _endPointsRepository["local"]; } }

        public void Send(string endPointName, IMessage message)
        {
            var endPoint = _endPointsRepository[endPointName];
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
            var topicEndPoint = _endPointsRepository[topicEndPointName];
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
