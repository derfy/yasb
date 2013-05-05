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

namespace Yasb.Redis.Messaging
{
    public delegate IRedisClient RedisClientFactory(EndPoint connection);

    public class RedisQueueFactory : AbstractQueueFactory<EndPoint>
    {
        private RedisClientFactory _redisClientFactory;
        private ISerializer _serializer;
        public RedisQueueFactory(QueueConfiguration<EndPoint> queueConfiguration, ISerializer serializer, RedisClientFactory redisClientFactory)
            :base(queueConfiguration)
        {
            _serializer = serializer;
            _redisClientFactory = redisClientFactory;
        }

        public override IQueue CreateFromEndPointValue(string endPointValue)
        {
            var array = endPointValue.Split(':');
            if (array.Length != 3)
                throw new ApplicationException("endPoint is not valid");
            var ipAddress = IPAddress.Parse(array[0]);
            var port = int.Parse(array[1]);
            return CreateQueue(new IPEndPoint(ipAddress, port), array[2]);
        }

        protected override IQueue CreateQueue(EndPoint connection, string queueName)
        {
            var redisClient = _redisClientFactory(connection);
            return new RedisQueue(queueName, _serializer, redisClient);
        }

    }
}
