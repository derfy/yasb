﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections.Concurrent;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Common.Messaging;
using Yasb.Common.Serialization;
using System.Threading;
using Yasb.Redis.Messaging.Scripts;
using Yasb.Common.Extensions;
using System.Net;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints.Redis;

namespace Yasb.Redis.Messaging
{

    public class RedisQueue : IQueue<RedisEndPoint>, IDisposable
    {
        private ISerializer _serializer;
        private IRedisClient _redisClient;
        private string _queueName;
        public RedisQueue(RedisEndPoint localEndPoint, ISerializer serializer, IRedisClient redisClient)
        {
            _serializer = serializer;
            _queueName = localEndPoint.QueueName;
            LocalEndPoint=localEndPoint;
            _redisClient = redisClient;
          
        }


        public bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            envelope = null;
            var bytes = _redisClient.EvalSha("TryGetEnvelope.lua", 1, _queueName, now.Subtract(timoutWindow).Ticks.ToString(), now.Ticks.ToString());
            if (bytes == null)
                return false;
            envelope = _serializer.Deserialize<MessageEnvelope>(bytes);
            return true;
        }

        public void SetMessageCompleted(string envelopeId, DateTime now)
        {
            _redisClient.EvalSha("SetMessageCompleted.lua", 1,envelopeId,DateTime.UtcNow.ToString());
        }

        public void SetMessageInError(string envelopeId,string errorMessage)
        {
            _redisClient.EvalSha("SetMessageInError.lua", 1, envelopeId, errorMessage);
        }

        public void Push(MessageEnvelope envelope)
        {
             envelope.Id = Guid.NewGuid().ToString();
             var bytes = _serializer.Serialize(envelope);
             _redisClient.EvalSha("PushMessage.lua", 1, _queueName.ToUtf8Bytes(), bytes);
        }

        public RedisEndPoint LocalEndPoint { get; private set; }

        public void Clear() {
            _redisClient.Del(_queueName);
        }

        public void Dispose()
        { }


    }
}
