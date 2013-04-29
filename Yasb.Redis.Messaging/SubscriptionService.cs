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
        private string _queueName;
        public SubscriptionService(IRedisClient connection,string queueName)
        {
            _queueName = queueName;
           _connection = connection;
        }
        public string[] GetSubscriptionEndPoints(string typeName)
        {
            string set = string.Format("{0}:{1}", _queueName, typeName);
            return _connection.SMembers(set).Select(e => e.FromUtf8Bytes()).ToArray();
            
        }


        public void Subscribe(string typeName, string subscriberEndPoint)
        {
            string set = string.Format("{0}:{1}", _queueName, typeName);
            _connection.Sadd(set, subscriberEndPoint);
        }


        public void UnSubscribe(string typeName, string subscriberEndPoint)
        {
            string set = string.Format("{0}:{1}", _queueName, typeName);
            _connection.SRem(set, subscriberEndPoint);
        }

        public void Dispose()
        {
        }
    }
}
