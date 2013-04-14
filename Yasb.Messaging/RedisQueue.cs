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

namespace Yasb.Redis.Messaging
{

    public class RedisQueue : IQueue, IDisposable
    {
        private ConcurrentDictionary<string, byte[]> _internalCache = new ConcurrentDictionary<string, byte[]>();
        private ScriptsCache _scriptsCache;
        private ISerializer _serializer;
        private BusEndPoint _localEndPoint;

        private RedisClient Connection { get { return _scriptsCache.Connection; } }
        public RedisQueue(BusEndPoint localEndPoint, ISerializer serializer, ScriptsCache scriptsCache)
        {
            _serializer = serializer;
            _localEndPoint = localEndPoint;
            _scriptsCache = scriptsCache;
            
        }
        public BusEndPoint LocalEndPoint
        {
            get { return _localEndPoint; }
        }
        public bool TryGetEnvelope(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            _scriptsCache.EnsureScriptCached("TryGetEnvelope.lua", GetType());
            envelope = null;
            var bytes = _scriptsCache.EvalSha("TryGetEnvelope.lua", 1, _localEndPoint.QueueName, now.Subtract(timoutWindow).Ticks.ToString(), now.Ticks.ToString());
            if (bytes == null)
                return false;
            envelope = _serializer.Deserialize<MessageEnvelope>(bytes);
            return true;
        }
       
       


        public void SetMessageCompleted(string envelopeId)
        {
            _scriptsCache.EnsureScriptCached( "SetMessageCompleted.lua" , GetType());
            _scriptsCache.EvalSha("SetMessageCompleted.lua", 1, envelopeId, DateTime.Now.Ticks.ToString());
        }


        public void Push(MessageEnvelope envelope)
        {
            envelope.Id = Guid.NewGuid().ToString();
            var bytes = _serializer.Serialize(envelope);
            Connection.LPush(_localEndPoint.QueueName, bytes);
        }



        public void Dispose()
        { }









       
    }
}
