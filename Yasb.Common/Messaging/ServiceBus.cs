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
        private readonly IWorkerPool<MessageEnvelope> _messageReveiverWorkerPool;
        private readonly ISubscriptionService<TEndPoint> _subscriptionService;
        private IEndPointsRepository<TEndPoint> _endPointsRepository;
        public ServiceBus(IEndPointsRepository<TEndPoint> endPointsRepository, IQueueFactory<TEndPoint> queueFactory, ISubscriptionService<TEndPoint> subscriptionService, IWorkerPool<MessageEnvelope> messageReveiverWorkerPool)
        {
            _endPointsRepository = endPointsRepository;
            _queueFactory = queueFactory;
            _subscriptionService = subscriptionService;
            _messageReveiverWorkerPool = messageReveiverWorkerPool;
        }

        public virtual TEndPoint LocalEndPoint { get { return _endPointsRepository["local"]; } }

        public void Send(string endPointName, IMessage message)
        {
            var endPoint = _endPointsRepository[endPointName];
            var queue = _queueFactory.CreateQueue(endPoint);
            queue.Push(new MessageEnvelope(message));
        }
       
        public void Publish(IMessage message) 
        {
            var subscriptions = _subscriptionService.GetSubscriptionEndPoints();
            foreach (var subscriptionEndPoint in subscriptions)
            {
                var queue = _queueFactory.CreateQueue(subscriptionEndPoint);
                queue.Push(new MessageEnvelope( message));
            }
            
        }

        public void Subscribe<TMessage>(string topicEndPointName) where TMessage : IMessage
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
