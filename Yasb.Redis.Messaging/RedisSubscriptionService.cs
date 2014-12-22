using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Extensions;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Net;
using Yasb.Redis.Messaging.Serialization;
using Yasb.Common.Messaging.Serialization;
using Yasb.Common.Messaging.Serialization.Json;
using Yasb.Redis.Messaging.Configuration;

namespace Yasb.Redis.Messaging
{
    public class RedisSubscriptionService : ISubscriptionService<RedisEndPointConfiguration>
    {
        private IRedisClient _connection;
        private AbstractJsonSerializer<RedisEndPointConfiguration> _serializer;
       // private byte[] _localEndPointValue;
        private RedisEndPointConfiguration _localEndPoint;
        public RedisSubscriptionService(RedisSubscriptionServiceConfiguration configuration, RedisClientFactory clientFactory, AbstractJsonSerializer<RedisEndPointConfiguration> serializer)
        {
           
           _serializer = serializer;
           _connection = clientFactory(configuration.Built);
           _localEndPoint = configuration.LocalEndPointConfiguration;
            
        }



        public void SubscribeTo(RedisEndPointConfiguration topicEndPoint)
        {
            var localEndPointValue = _serializer.Serialize(_localEndPoint);
            var topicEndPointValue = _serializer.Serialize(topicEndPoint);
            _connection.Sadd(topicEndPointValue, localEndPointValue);
        }

        public RedisEndPointConfiguration[] GetSubscriptionEndPoints()
        {
            var localEndPointValue = _serializer.Serialize(_localEndPoint);
            return _connection.SMembers(localEndPointValue).Select(e => _serializer.Deserialize(e)).ToArray();
        }

        public void UnSubscribe(string topicName, RedisEndPointConfiguration subscriberEndPoint)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

    }
}
