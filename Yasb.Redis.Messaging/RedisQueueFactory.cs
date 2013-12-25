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
using Yasb.Common.Messaging.EndPoints.Redis;

namespace Yasb.Redis.Messaging
{
    public delegate IRedisClient RedisClientFactory(RedisEndPoint connection);

    public class RedisQueueFactory :  IQueueFactory<RedisEndPoint>
    {
        private RedisClientFactory _redisClientFactory;
        private ISerializer _serializer;

        public RedisQueueFactory(ISerializer serializer, RedisClientFactory redisClientFactory)
        {
            _serializer = serializer;
            _redisClientFactory = redisClientFactory;
        }

        public  IQueue<RedisEndPoint> CreateQueue(RedisEndPoint endPoint)
        {
            var redisClient = _redisClientFactory(endPoint);
            return new RedisQueue(endPoint, _serializer, redisClient);
        }
    }
}
