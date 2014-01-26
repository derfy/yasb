using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Net;
using Yasb.Common.Messaging.Configuration;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Redis.Messaging.Serialization;
using Yasb.Common.Messaging.Serialization;
using Yasb.Common.Messaging.Serialization.Json;
using Yasb.Redis.Messaging.Configuration;

namespace Yasb.Redis.Messaging
{
    public delegate IRedisClient RedisClientFactory(EndPoint address);

    public class RedisQueueFactory :  IQueueFactory<RedisEndPointConfiguration>
    {
        private RedisClientFactory _redisClientFactory;
        private AbstractJsonSerializer<MessageEnvelope> _serializer;

        public RedisQueueFactory(AbstractJsonSerializer<MessageEnvelope> serializer, RedisClientFactory redisClientFactory)
        {
            _serializer = serializer;
            _redisClientFactory = redisClientFactory;
        }

        public IQueue<RedisEndPointConfiguration> CreateQueue(RedisEndPointConfiguration endPoint)
        {
            var redisClient = _redisClientFactory(endPoint.Built.Address);
            return new RedisQueue(endPoint, _serializer, redisClient);
        }
    }
}
