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
        private Func<AddressInfo,RedisClient> _connectionFactory;
        private BusEndPoint _localEndPoint;
        public SubscriptionService(BusEndPoint localEndPoint, Func<AddressInfo,RedisClient> connectionFactory)
        {
            _localEndPoint = localEndPoint;
            _connectionFactory = connectionFactory;
        }
        public BusEndPoint[] GetSubscriptionEndPoints(string typeName)
        {
            string set = string.Format("{0}:{1}", _localEndPoint.ToString(), typeName);
            using (var conn = _connectionFactory(_localEndPoint.AddressInfo))
            {
                return conn.SMembers(set).Select(e => BusEndPoint.Parse(e.FromUtf8Bytes())).ToArray();
            }
            
        }


        public void AddSubscriberFor(string typeName, BusEndPoint subscriberEndPoint) 
        {
            string set = string.Format("{0}:{1}", _localEndPoint.ToString(), typeName);
            using (var conn = _connectionFactory(_localEndPoint.AddressInfo))
            {
                conn.Sadd(set, subscriberEndPoint.ToString());
            }
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
