using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Extensions;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Net;
using Yasb.Common.Messaging.Connections;

namespace Yasb.Redis.Messaging
{
    public class RedisSubscriptionMessageHandler : ISubscriptionService<RedisConnection>, IHandleMessages<SubscriptionMessage<RedisConnection>>
    {
        private IRedisClient _connection;
      
        public RedisSubscriptionMessageHandler(IRedisClient connection)
        {
           _connection = connection;
        }
       


        public void Handle(SubscriptionMessage<RedisConnection> message)
        {
            foreach (var subscription in message.Subscriptions)
            {
                _connection.Sadd(message.TypeName, subscription.Value);
            }
        }

        public SubscriptionInfo<RedisConnection>[] GetSubscriptions(string typeName)
        {
            return _connection.SMembers(typeName).Select(e => Parse(e.FromUtf8Bytes())).ToArray();
        }
        

        public void UnSubscribe(string typeName, string subscriberEndPoint)
        {
            _connection.SRem(typeName, subscriberEndPoint);
        }

        public void Dispose()
        {
        }

        private SubscriptionInfo<RedisConnection> Parse(string subscriptionValue)
        {
            var array = subscriptionValue.Split(':');
            if (array.Length < 4)
                throw new ApplicationException(string.Format("{0} is not a valid Subscription", subscriptionValue));
            var ipAddress = IPAddress.Parse(array[0]);
            var port = int.Parse(array[1]);
            var queueName = array[2];
            var endPoint = new RedisQueueEndPoint(new RedisConnection(array[0], port), queueName);
            return new SubscriptionInfo<RedisConnection>(endPoint, array[3]);
        }


      
    }
}
