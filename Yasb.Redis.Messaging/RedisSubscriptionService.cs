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
    public class RedisSubscriptionService<TEndPoint> : ISubscriptionService<TEndPoint>
    {
        private IRedisClient _redisClient;
        private AbstractJsonSerializer<TEndPoint> _serializer;
       // private byte[] _localEndPointValue;
        private TEndPoint _localEndPoint;
        public RedisSubscriptionService(TEndPoint localEndPoint, IRedisClient redisClient, AbstractJsonSerializer<TEndPoint> serializer)
        {
           
           _serializer = serializer;
           _redisClient = redisClient;
           _localEndPoint = localEndPoint;
            
        }



        public void SubscribeTo(TEndPoint topicEndPoint)
        {
            var localEndPointValue = _serializer.Serialize(_localEndPoint);
            var topicEndPointValue = _serializer.Serialize(topicEndPoint);
            _redisClient.Sadd(topicEndPointValue, localEndPointValue);
        }

        public TEndPoint[] GetSubscriptionEndPoints()
        {
            var localEndPointValue = _serializer.Serialize(_localEndPoint);
            return _redisClient.SMembers(localEndPointValue).Select(e => _serializer.Deserialize(e)).ToArray();
        }

        public void UnSubscribe(string topicName, TEndPoint subscriberEndPoint)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

    }
}
