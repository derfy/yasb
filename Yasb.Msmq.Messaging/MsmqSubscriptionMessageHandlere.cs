using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Collections.Concurrent;
using Yasb.Common.Messaging.EndPoints.Msmq;

namespace Yasb.Msmq.Messaging
{
    public class MsmqSubscriptionService : ISubscriptionService<MsmqEndPoint>
    {
        private MsmqEndPoint _localEndPoint;
        private ConcurrentDictionary<MsmqEndPoint, List<MsmqEndPoint>> _subscriptionsStore = new ConcurrentDictionary<MsmqEndPoint, List<MsmqEndPoint>>();
        public MsmqSubscriptionService(MsmqEndPoint localEndPoint)
        {
            _localEndPoint = localEndPoint;
        }



        public void SubscribeTo(MsmqEndPoint topicEndPoint)
        {
            List<MsmqEndPoint> subscribers = null;
            //if (!_subscriptionsStore.TryGetValue(message.TopicEndPoint, out subscribers))
            //{
            //    subscribers = new List<string>();
            //    _subscriptionsStore.TryAdd(message.TopicEndPoint, subscribers);
            //}
            //subscribers.Add(message.SubscriberEndPoint);
        }
      

        public MsmqEndPoint[] GetSubscriptionEndPoints()
        {
            List<MsmqEndPoint> subscribers = null;
            if (!_subscriptionsStore.TryGetValue(_localEndPoint, out subscribers))
            {
                return new MsmqEndPoint[] { };
            }
            return subscribers.ToArray();
        }

        public void UnSubscribe(string topicName, MsmqEndPoint subscriberEndPoint)
        {
            throw new NotImplementedException();
        }


       
    }
}
