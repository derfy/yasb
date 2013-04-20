using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Extensions;
using Yasb.Redis.Messaging.Client.Interfaces;

namespace Yasb.Redis.Messaging
{
    public class SubscriptionService : ISubscriptionService
    {
        private IRedisClient _connection;
        private BusEndPoint _localEndPoint;
        public SubscriptionService(BusEndPoint localEndPoint, IRedisClient connection)
        {
            _localEndPoint = localEndPoint;
            _connection = connection;
        }
        public BusEndPoint[] GetSubscriptionEndPoints(string typeName)
        {
            string set = string.Format("{0}:{1}", _localEndPoint.Value, typeName);
            return _connection.SMembers(set).Select(e => {
                return new BusEndPoint(e.FromUtf8Bytes()); 
            }).ToArray();
            
        }


        public void AddSubscriberFor(string typeName, BusEndPoint subscriberEndPoint) 
        {
            string set = string.Format("{0}:{1}", _localEndPoint.Value, typeName);
            _connection.Sadd(set, subscriberEndPoint.Value);
        }



        public void RemoveSubscriberFor(string typeName, BusEndPoint subscriberEndPoint)
        {
            string combinedValue = string.Format("{0}:{1}", subscriberEndPoint.Value, typeName);
        }

        public void Dispose()
        {
         }
    }
}
