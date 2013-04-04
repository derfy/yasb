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
        private RedisEndPoint _endPoint;
        public RedisQueue(RedisEndPoint endPoint, ISerializer serializer,  ScriptsCache scriptsCache)
        {
            _serializer = serializer;
            _endPoint = endPoint;
            _scriptsCache = scriptsCache;
            
        }
        
        public bool TryGetEnvelope(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            _scriptsCache.EnsureScriptCached("TryGetEnvelope.lua", GetType());
            envelope = null;
            var bytes = _scriptsCache.EvalSha("TryGetEnvelope.lua", 1, _endPoint.QueueName, now.Subtract(timoutWindow).Ticks.ToString(), now.Ticks.ToString());
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
            var bytes = _serializer.Serialize(envelope);
            Connection.LPush(_endPoint.QueueName, bytes);
        }


        public MessageEnvelope WrapInEnvelope(IMessage message, IEndPoint fromEndPoint)
        {
            return new MessageEnvelope(message, Guid.NewGuid().ToString(), fromEndPoint, LocalEndPoint);
        }
        public IEndPoint LocalEndPoint
        {
            get { return _endPoint; }
        }






        public void Dispose()
        { }
        
        private RedisClient Connection { get { return _scriptsCache.Connection; } }




        
    }
}
