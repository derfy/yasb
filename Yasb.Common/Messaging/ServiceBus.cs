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
        private readonly IQueueFactory _queueFactory;
        private readonly ITaskRunner _taskRunner;
        private readonly ISubscriptionService _subscriptionService;
        public ServiceBus(IWorker messagesReceiver, IQueueFactory queueFactory, ISubscriptionService subscriptionService, ITaskRunner taskRunner)
        {
            _queueFactory = queueFactory;
            _messagesReceiver = messagesReceiver;
            _taskRunner = taskRunner;
            _subscriptionService = subscriptionService;
        }

        public virtual string LocalEndPoint 
        {
            get
            {
                var localQueue = _queueFactory.CreateFromEndPointName("local");
                return localQueue.LocalEndPoint;
            } 
        }
        
        public void Publish(IMessage message) 
        {
            var subscriptionEndPoints = _subscriptionService.GetSubscriptionEndPoints(message.GetType().FullName);
            foreach (var endPointValue in subscriptionEndPoints)
            {
                IQueue queue = _queueFactory.CreateFromEndPointValue(endPointValue);
                Send(queue, message);
            }
            
        }
        public void Send(string endPointName, IMessage message)
        {
            var queue = _queueFactory.CreateFromEndPointName(endPointName);
            Send(queue, message);
        }
        public void Subscribe<TMessage>(string endPointName) where TMessage : IMessage
        {
            var queue = _queueFactory.CreateFromEndPointName(endPointName);
            Subscribe<TMessage>(queue);
        }


        public void Subscribe<TMessage>(IQueue queue) where TMessage : IMessage
        {
            var subscriptionMessage = new SubscriptionMessage(typeof(TMessage).FullName,LocalEndPoint);
            Send(queue, subscriptionMessage);
        }

        public void Send(IQueue queue, IMessage message)
        {
            queue.Push(message, LocalEndPoint);
        }
        
        public void Run()
        {
            _taskRunner.Run(_messagesReceiver.Execute,_messagesReceiver.OnException);
        }

       

        public void Dispose()
        {
        }
    }
}
