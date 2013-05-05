using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Collections.Concurrent;

namespace Yasb.Msmq.Messaging
{
    public class MsmqSubscriptionService : ISubscriptionService
    {
        private string _queueName;
        private ConcurrentDictionary<string, List<string>> _subscriptionsStore = new ConcurrentDictionary<string, List<string>>();
        public MsmqSubscriptionService(string queueName)
        {
            this._queueName = queueName;
        }

       
        public string[] GetSubscriptionEndPoints(string typeName)
        {
            string set = string.Format("{0}:{1}", _queueName, typeName);
            List<string> subscribers = null;
            if (!_subscriptionsStore.TryGetValue(set, out subscribers))
            {
                return new string[]{} ;
            }
            return subscribers.ToArray();
        }

        public void Subscribe(string typeName, string subscriberEndPoint)
        {
            string set = string.Format("{0}:{1}", _queueName, typeName);
            List<string>subscribers = null;
            if (!_subscriptionsStore.TryGetValue(set,out subscribers))
            {
                subscribers = new List<string>();
                _subscriptionsStore.TryAdd(set, subscribers);
            }
            subscribers.Add(subscriberEndPoint);
        }

        public void UnSubscribe(string typeName, string subscriberEndPoint)
        {
            throw new NotImplementedException();
        }




        
    }
}
