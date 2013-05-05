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
      
        public SubscriptionService(IRedisClient connection)
        {
           _connection = connection;
        }
        public string[] GetSubscriptionEndPoints(string typeName)
        {
             return _connection.SMembers(typeName).Select(e => e.FromUtf8Bytes()).ToArray();
        }


        public void Subscribe(string typeName, string subscriberEndPoint)
        {
            _connection.Sadd(typeName, subscriberEndPoint);
        }


        public void UnSubscribe(string typeName, string subscriberEndPoint)
        {
            _connection.SRem(typeName, subscriberEndPoint);
        }

        public void Dispose()
        {
        }
    }
}
