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
   
    public class Queue : IQueue
    {
        private ConcurrentDictionary<string, byte[]> _internalCache = new ConcurrentDictionary<string, byte[]>();
        private RedisClient _conn;
        private ISerializer _serializer;
        private string _queueName;
        public Queue(RedisClient conn,ISerializer serializer,string queueName)
        {
            _serializer = serializer;
            _queueName = queueName;
            _conn = conn;
        }
        public void Initialize(){
            var fileNames = new string[] { "GetMessage.lua", "SetMessageInProgress.lua", "SetMessageError.lua", "SetMessageCompleted.lua" };
            var type=this.GetType();
            foreach (var fileName in fileNames)
            {
                using (StreamReader reader = new StreamReader(type.Assembly.GetManifestResourceStream(string.Format("{0}.Scripts.{1}", type.Namespace,fileName))))
                {
                    string fileContent = reader.ReadToEnd();
                    _internalCache[fileName]= _conn.Load(fileContent);
                }
            }
        }
        public MessageEnvelope GetMessage(TimeSpan delta)
        {
            var bytes = EvalSha("GetMessage.lua", 1, _queueName, DateTime.Now.Subtract(delta).Ticks.ToString());
            if (bytes == null)
                return null;
            return _serializer.Deserialize<MessageEnvelope>(bytes);
        }
        public bool TrySetMessageInProgress(Guid envelopeId)
        {
            return EvalSha("SetMessageInProgress.lua", 1, envelopeId.ToString(), DateTime.Now.Ticks.ToString()) != null;
        }


        public void SetMessageCompleted(Guid envelopeId)
        {
            EvalSha("SetMessageCompleted.lua", 1, envelopeId.ToString(), DateTime.Now.Ticks.ToString());
        }



        public void SetMessageError(Guid envelopeId)
        {
            EvalSha("SetMessageError.lua", 1, envelopeId.ToString());
        }


        public void Push(MessageEnvelope envelope)
        {
            var bytes = _serializer.Serialize(envelope);
            _conn.LPush(_queueName, bytes);
        }

        private byte[] EvalSha(string fileName,int noKeys, params string[] keys)
        {
            var scriptSha = _internalCache[fileName];
            return _conn.EvalSha(scriptSha, noKeys, keys);
        }





    }
}
