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

namespace Yasb.Redis.Messaging.Client
{
    public class RedisClient : IRedisClient, IDisposable
    {
        internal const int Success = 1;
        private RedisConnectionManager _socketClient;
        private ConcurrentDictionary<string, byte[]> _internalCache = new ConcurrentDictionary<string, byte[]>();
        public RedisClient(RedisConnectionManager socketClient)
        {
           _socketClient = socketClient;
        }

        public EndPoint Address
        {
            get { return _socketClient.Address; }
        }

        private byte[] Load(string script){
            
            return SendCommand<byte[]>(RedisCommand.Load(script));
        }

        public virtual byte[] EvalSha(string fileName, int noKeys, params string[] keys)
        {
            byte[] scriptSha = null;
            while(!_internalCache.TryGetValue(fileName,out scriptSha))
            {
                EnsureScriptIsCached(fileName);
            }
            return EvalSha(scriptSha, noKeys, keys);
        }


       


        public byte[] LPush(string listId, byte[] value)
        {
            if (listId == null)
                throw new ArgumentNullException("listId");
            if (value == null)
                throw new ArgumentNullException("value");
            return SendCommand<byte[]>(RedisCommand.LPush(listId, value));
        }


       
        public byte[] Sadd(string set, string value)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[]>(RedisCommand.SAdd(set, value));

        }

        public byte[] SRem(string set, string value)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[]>(RedisCommand.SRem(set, value));
        }
        public byte[][] SMembers(string set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[][]>(RedisCommand.SMembers(set));
        }



        private byte[] EvalSha(byte[] scriptSha, int noKeys, params string[] keys)
        {
            var mergedArray = new byte[keys.Length + 1][];

            var multiByteKeys = keys.ToMultiByteArray();

            mergedArray[0] = noKeys.ToUtf8Bytes();
            for (int i = 0; i < keys.Length; i++)
            {
                mergedArray[i + 1] = multiByteKeys[i];
            }
            return SendCommand<byte[]>(RedisCommand.EvalSha(scriptSha, mergedArray));
        }

        private void EnsureScriptIsCached(string fileName)
        {
            var type =typeof(RedisScriptsProbe);
            using (StreamReader reader = new StreamReader(type.Assembly.GetManifestResourceStream(string.Format("{0}.{1}", type.Namespace, fileName))))
            {
                var scriptSha=SendCommand<byte[]>(RedisCommand.Load(reader.ReadToEnd()));
                _internalCache.TryAdd(fileName, scriptSha);
            }
        }
        private TResult SendCommand<TResult>(IProcessResult<TResult> command)
        {
          
            var taskConnect = _socketClient.StartConnect();
            taskConnect.Wait();
            var taskSend = _socketClient.SendAsync(command.ToBinary,taskConnect.Result);
            using (var commandProcessor = new CommandResultProcessor(taskSend.Result))
            {
                return command.ProcessResponse(commandProcessor);
            }
        }

        private bool isDisposed = false;
      
        public void Dispose()
        {
            isDisposed = true;
        }





       
    }
}
