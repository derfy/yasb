using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Messaging
{
    public class ServiceBus<TConnection> : IServiceBus<TConnection> 
    {
        private readonly AbstractQueueFactory<TConnection> _queueFactory;
        private readonly IWorkerPool<MessageEnvelope> _messageReveiverWorkerPool;
        private readonly ISubscriptionService<TConnection> _subscriptionService;
        private Func<Type, IEnumerable<IHandleMessages>> _messageHandlerFactory;

        public ServiceBus(AbstractQueueFactory<TConnection> queueFactory, ISubscriptionService<TConnection> subscriptionService, IWorkerPool<MessageEnvelope> messageReveiverWorkerPool, Func<Type, IEnumerable<IHandleMessages>> messageHandlerFactory)
        {
            _queueFactory = queueFactory;
            _subscriptionService = subscriptionService;
            _messageReveiverWorkerPool = messageReveiverWorkerPool;
            _messageHandlerFactory = messageHandlerFactory;
        }

        public virtual QueueEndPoint<TConnection> LocalEndPoint 
        {
            get
            {
                var localQueue = _queueFactory.CreateFromEndPointName("local");
                return localQueue.LocalEndPoint;
            } 
        }
        public void Send(string endPointName, IMessage message)
        {
            var queue = _queueFactory.CreateFromEndPointName(endPointName);
            var handlerType = typeof(NullMessageHandler<>).MakeGenericType(message.GetType());
            var envelope = queue.CreateMessageEnvelope(message, LocalEndPoint, handlerType.AssemblyQualifiedName);
            queue.Push(envelope);
        }
        public void Publish(IMessage message) 
        {
            var subscriptions = _subscriptionService.GetSubscriptions(message.GetType().AssemblyQualifiedName);
            foreach (var subscription in subscriptions)
            {
                IQueue<TConnection> queue = _queueFactory.CreateQueue(subscription.EndPoint.Connection, subscription.EndPoint.Name);
                var envelope = queue.CreateMessageEnvelope(message, LocalEndPoint, subscription.Handler);
                queue.Push(envelope);
            }
            
        }
        
        public void Subscribe<TMessage>(string endPointName) where TMessage : IMessage
        {
            var queue = _queueFactory.CreateFromEndPointName(endPointName);
            Subscribe<TMessage>(queue);
        }


        public void Subscribe<TMessage>(IQueue<TConnection> queue) where TMessage : IMessage
        {
            var subscriptions = _messageHandlerFactory(typeof(TMessage)).Select(h=>new SubscriptionInfo<TConnection>(LocalEndPoint, h.GetType().AssemblyQualifiedName));
            var subscriptionMessage = new SubscriptionMessage<TConnection>(typeof(TMessage).AssemblyQualifiedName, subscriptions);
            var envelope = queue.CreateMessageEnvelope(subscriptionMessage, LocalEndPoint, _subscriptionService.GetType().AssemblyQualifiedName);
            queue.Push(envelope);
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
