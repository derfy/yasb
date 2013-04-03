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

namespace Yasb.Redis.Messaging
{

    public class RedisQueue : IQueue, IDisposable
    {
        private ConcurrentDictionary<string, byte[]> _internalCache = new ConcurrentDictionary<string, byte[]>();
        private RedisClient _connection;
        private ISerializer _serializer;
        private RedisEndPoint _endPoint;
        public RedisQueue(RedisEndPoint endPoint, ISerializer serializer,  RedisClient connection)
        {
            _serializer = serializer;
            _endPoint = endPoint;
            _connection = connection;
        }
        public void Initialize(){
            var fileNames = new string[] { "TryGetEnvelope.lua", "SetMessageCompleted.lua" };
            var type=this.GetType();
            foreach (var fileName in fileNames)
            {
                using (StreamReader reader = new StreamReader(type.Assembly.GetManifestResourceStream(string.Format("{0}.Scripts.{1}", type.Namespace,fileName))))
                {
                    string fileContent = reader.ReadToEnd();
                    _internalCache[fileName] = _connection.Load(fileContent);
                }
            }
        }
        public bool TryGetEnvelope(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            envelope = null;
            var bytes = EvalSha("TryGetEnvelope.lua", 1, _endPoint.QueueName, now.Subtract(timoutWindow).Ticks.ToString(), now.Ticks.ToString());
            if (bytes == null)
                return false;
            envelope = _serializer.Deserialize<MessageEnvelope>(bytes);
            return true;
        }
       
       


        public void SetMessageCompleted(string envelopeId)
        {
            EvalSha("SetMessageCompleted.lua", 1, envelopeId, DateTime.Now.Ticks.ToString());
        }


        public void Push(MessageEnvelope envelope)
        {
            var bytes = _serializer.Serialize(envelope);
            _connection.LPush(_endPoint.QueueName, bytes);
        }


        public MessageEnvelope WrapInEnvelope(IMessage message, IEndPoint fromEndPoint)
        {
            return new MessageEnvelope(message, Guid.NewGuid().ToString(), fromEndPoint, LocalEndPoint);
        }

        private byte[] EvalSha(string fileName,int noKeys, params string[] keys)
        {
            var scriptSha = _internalCache[fileName];
            return _connection.EvalSha(scriptSha, noKeys, keys);
        }




        public IEndPoint LocalEndPoint
        {
            get { return _endPoint; }
        }
        public void Dispose()
        {
        }







        
    }
}
