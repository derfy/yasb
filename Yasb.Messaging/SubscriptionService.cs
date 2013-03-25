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
        private IEndPoint _localEndPoint;
        public SubscriptionService(IEndPoint localEndPoint, RedisClient connection)
        {
            _localEndPoint = localEndPoint;
            _connection = connection;
        }
        public IEndPoint[] GetSubscriptionEndPoints(string typeName)
        {
            string set = string.Format("{0}:{1}", _localEndPoint.Value, typeName);
            return _connection.SMembers(set).Select(e => RedisEndPoint.Parse(e.FromUtf8Bytes())).ToArray();
            
        }


        public void AddSubscriberFor(string typeName, IEndPoint subscriberEndPoint) 
        {
            string set = string.Format("{0}:{1}", _localEndPoint.Value, typeName);
            _connection.Sadd(set, subscriberEndPoint.Value);
        }



        public void RemoveSubscriberFor(string typeName, IEndPoint subscriberEndPoint)
        {
            string combinedValue = string.Format("{0}:{1}", subscriberEndPoint.Value, typeName);
        }

        public void Dispose()
        {
         }
    }
}
