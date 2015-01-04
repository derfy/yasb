using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using Yasb.Common.Extensions;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Common.Messaging;
using System.Collections.Concurrent;
using Yasb.Redis.Messaging.Scripts;
using Yasb.Redis.Messaging.Client.Commands;
using Yasb.Common.Messaging.Tcp;

namespace Yasb.Redis.Messaging.Client
{
    public class RedisClient : IRedisClient, IDisposable
    {
        internal const int Success = 1;
        private TcpConnectionsManager<RedisConnection> _connectionsManager;
        private ConcurrentDictionary<string, byte[]> _internalCache = new ConcurrentDictionary<string, byte[]>();
        public RedisClient(TcpConnectionsManager<RedisConnection> connectionsManager)
        {
            _connectionsManager = connectionsManager;
        }

        

        public virtual byte[] EvalSha(string fileName, int noKeys, params string[] keys)
        {
            
            return EvalSha(fileName, noKeys, keys.ToMultiByteArray());
        }

        public virtual byte[] EvalSha(string fileName, int noKeys, params byte[][] keys)
        {
            byte[] scriptSha = null;
            while (!_internalCache.TryGetValue(fileName, out scriptSha))
            {
                EnsureScriptIsCached(fileName);
            }
            return EvalSha(scriptSha, noKeys, keys);
        }


        public byte[] Del(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return SendCommand<byte[]>(RedisCommandFactory.Del(key));
        }
       


        public byte[] LPush(string listId, byte[] value)
        {
            if (listId == null)
                throw new ArgumentNullException("listId");
            if (value == null)
                throw new ArgumentNullException("value");
            return SendCommand<byte[]>(RedisCommandFactory.LPush(listId, value));
        }



        public byte[] Sadd(byte[] set, byte[] value)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[]>(RedisCommandFactory.SAdd(set, value));

        }

        public byte[] SRem(string set, string value)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[]>(RedisCommandFactory.SRem(set, value));
        }

        public byte[][] SMembers(byte[] set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[][]>(RedisCommandFactory.SMembers(set));
        }



        private byte[] EvalSha(byte[] scriptSha, int noKeys, params byte[][] multiByteKeys)
        {
            var mergedArray = new byte[multiByteKeys.Length + 1][];

            mergedArray[0] = noKeys.ToUtf8Bytes();
            for (int i = 0; i < multiByteKeys.Length; i++)
            {
                mergedArray[i + 1] = multiByteKeys[i];
            }
            return SendCommand<byte[]>(RedisCommandFactory.EvalSha(scriptSha, mergedArray));
        }

        private void EnsureScriptIsCached(string fileName)
        {
            var type =typeof(RedisScriptsProbe);
            using (StreamReader reader = new StreamReader(type.Assembly.GetManifestResourceStream(string.Format("{0}.{1}", type.Namespace, fileName))))
            {
                var scriptSha = SendCommand<byte[]>(RedisCommandFactory.Load(reader.ReadToEnd()));
                _internalCache.TryAdd(fileName, scriptSha);
            }
        }

        private TResult SendCommand<TResult>(IRedisCommand<TResult> command)
        {
           
            var task = _connectionsManager.SendAsync(command.ToBinary);
            return task.ContinueWith(taskSend =>
            {
                using (var commandProcessor = new CommandResultProcessor(taskSend.Result))
                {
                    return command.ProcessResponse(commandProcessor);
                }
            }).Result;                         
            
        }

      
        public void Dispose()
        {
        }

       
    }
}
