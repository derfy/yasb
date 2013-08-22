using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Collections.Concurrent;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Msmq.Messaging
{
    public class MsmqSubscriptionMessageHandler : ISubscriptionService<MsmqConnection>, IHandleMessages<SubscriptionMessage<MsmqConnection>>
    {
        private string _queueName;
        private ConcurrentDictionary<string, List<SubscriptionInfo<MsmqConnection>>> _subscriptionsStore = new ConcurrentDictionary<string, List<SubscriptionInfo<MsmqConnection>>>();
        public MsmqSubscriptionMessageHandler(string queueName)
        {
            this._queueName = queueName;
        }


       

        public void Handle(SubscriptionMessage<MsmqConnection> message)
        {
            string set = string.Format("{0}:{1}", _queueName, message.TypeName);
            List<SubscriptionInfo<MsmqConnection>> subscribers = null;
            if (!_subscriptionsStore.TryGetValue(set,out subscribers))
            {
                subscribers = new List<SubscriptionInfo<MsmqConnection>>();
                _subscriptionsStore.TryAdd(set, subscribers);
            }
            subscribers.AddRange(message.Subscriptions);
            
        }
        public SubscriptionInfo<MsmqConnection>[] GetSubscriptions(string typeName)
        {
            string set = string.Format("{0}:{1}", _queueName, typeName);
            List<SubscriptionInfo<MsmqConnection>> subscribers = null;
            if (!_subscriptionsStore.TryGetValue(set, out subscribers))
            {
                return new SubscriptionInfo<MsmqConnection>[] { };
            }
            return subscribers.ToArray();
        }
        public void UnSubscribe(string typeName, string subscriberEndPoint)
        {
            throw new NotImplementedException();
        }




        
    }
}
