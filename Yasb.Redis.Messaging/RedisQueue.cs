using System;
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
using Yasb.Common.Messaging.Connections;

namespace Yasb.Redis.Messaging
{

    public class RedisQueue : IQueue<RedisConnection>, IDisposable
    {
        private ISerializer _serializer;
        private IRedisClient _redisClient;

        public RedisQueue(QueueEndPoint<RedisConnection> localEndPoint, ISerializer serializer, IRedisClient redisClient)
        {
            _serializer = serializer;
            LocalEndPoint=localEndPoint;
            _redisClient = redisClient;
          
        }


        public bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            envelope = null;
            var bytes = _redisClient.EvalSha("TryGetEnvelope.lua", 1, LocalEndPoint.Name, now.Subtract(timoutWindow).Ticks.ToString(), now.Ticks.ToString());
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

        public void Push(IMessage message, string replyTo, string messageHandler)
        {
            var envelope= CreateMessageEnvelope(message, replyTo, messageHandler);
             var bytes = _serializer.Serialize(envelope);
            _redisClient.EvalSha("PushMessage.lua", 1, LocalEndPoint.Name.ToUtf8Bytes(), bytes);
        }
       
        public QueueEndPoint<RedisConnection> LocalEndPoint { get; private set; }

        public void Clear() {
            _redisClient.Del(LocalEndPoint.Name);
        }

        public void Dispose()
        { }



        private MessageEnvelope CreateMessageEnvelope(IMessage message, string replyTo, string messageHandler)
        {
            var envelopeId = Guid.NewGuid().ToString();
            return new MessageEnvelope(envelopeId, message, replyTo, LocalEndPoint.Value, messageHandler);
        }




       

    }
}
