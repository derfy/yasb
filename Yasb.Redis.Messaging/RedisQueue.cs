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

namespace Yasb.Redis.Messaging
{

    public class RedisQueue : IQueue, IDisposable
    {
        private ConcurrentDictionary<string, byte[]> _internalCache = new ConcurrentDictionary<string, byte[]>();
        private ScriptsCache _scriptsCache;
        private ISerializer _serializer;
        private string _queueName;

        private IRedisClient _connection;
        public RedisQueue(string queueName, ISerializer serializer, IRedisClient connection, ScriptsCache scriptsCache)
        {
            _serializer = serializer;
            _queueName = queueName;
            _connection = connection;
            _scriptsCache = scriptsCache;
        }



        public bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            envelope = null;
            var bytes = _scriptsCache.EvalSha("TryGetEnvelope.lua", 1, _queueName, now.Subtract(timoutWindow).Ticks.ToString(), now.Ticks.ToString());
            if (bytes == null)
                return false;
            envelope = _serializer.Deserialize<MessageEnvelope>(bytes);
            return true;
        }

        public void SetMessageCompleted(string envelopeId)
        {
            _scriptsCache.EvalSha("SetMessageCompleted.lua", 1, envelopeId, DateTime.Now.Ticks.ToString());
        }


        public void Push(MessageEnvelope envelope)
        {
            envelope.Id = Guid.NewGuid().ToString();
            var bytes = _serializer.Serialize(envelope);
            _connection.LPush(_queueName, bytes);
        }



        public void Dispose()
        { }









       
    }
}
