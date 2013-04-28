using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Extensions;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Net;

namespace Yasb.Redis.Messaging
{
    public class SubscriptionService : ISubscriptionService
    {
        private IRedisClient _connection;
        private string _localEndPoint;
        public SubscriptionService(IRedisClient connection,string localEndPoint)
        {
            _localEndPoint = localEndPoint;
           _connection = connection;
        }
        public string[] GetSubscriptionEndPoints(string typeName)
        {
            string set = string.Format("{0}:{1}", _localEndPoint, typeName);
            return _connection.SMembers(set).Select(e => e.FromUtf8Bytes()).ToArray();
            
        }


        public void AddSubscriberFor(string typeName, string subscriberEndPoint)
        {
            string set = string.Format("{0}:{1}", _localEndPoint, typeName);
            _connection.Sadd(set, subscriberEndPoint);
        }


        public void RemoveSubscriberFor(string typeName, string subscriberEndPoint)
        {
            string combinedValue = string.Format("{0}:{1}", subscriberEndPoint, typeName);
        }

        public void Dispose()
        {
        }
    }
}
