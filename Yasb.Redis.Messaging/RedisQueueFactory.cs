using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Net;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Serialization;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Common.Messaging.Connections;

namespace Yasb.Redis.Messaging
{
    public delegate IRedisClient RedisClientFactory(RedisConnection connection);

    public class RedisQueueFactory : AbstractQueueFactory<RedisConnection>
    {
        private RedisClientFactory _redisClientFactory;
        private ISerializer _serializer;
        public RedisQueueFactory(QueueConfiguration<RedisConnection> queueConfiguration, ISerializer serializer, RedisClientFactory redisClientFactory)
            :base(queueConfiguration)
        {
            _serializer = serializer;
            _redisClientFactory = redisClientFactory;
        }


        public override IQueue<RedisConnection> CreateQueue(RedisConnection connection, string queueName)
        {
            var redisClient = _redisClientFactory(connection);
            var queueEndPoint = new RedisQueueEndPoint(connection, queueName);
            return new RedisQueue(queueEndPoint, _serializer, redisClient);
        }

    }
}
