using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging
{
    public class SubscriptionService : ISubscriptionService
    {
        private RedisClient _connection;
        private BusEndPoint _localEndPoint;
        public SubscriptionService(BusEndPoint localEndPoint, RedisClient connection)
        {
            _localEndPoint = localEndPoint;
            _connection = connection;
        }
        public BusEndPoint[] GetSubscriptionEndPoints(string typeName)
        {
            string set = string.Format("{0}:{1}", _localEndPoint.ToString(), typeName);
            return _connection.SMembers(set).Select(e => BusEndPoint.Parse(e.FromUtf8Bytes())).ToArray();
            
        }


        public void AddSubscriberFor(string typeName, BusEndPoint subscriberEndPoint) 
        {
            string set = string.Format("{0}:{1}", _localEndPoint.ToString(), typeName);
            _connection.Sadd(set, subscriberEndPoint.ToString());
        }



        public void RemoveSubscriberFor(string typeName, BusEndPoint subscriberEndPoint)
        {
            string combinedValue = string.Format("{0}:{1}", subscriberEndPoint, typeName);
        }

        public void Dispose()
        {
         }
    }
}
