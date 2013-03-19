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

namespace Yasb.Redis.Messaging.Client
{
    public class RedisClient : IDisposable
    {
        internal const int Success = 1;
        private RedisSocket _socketClient;
        private AddressInfo _addressInfo;
        public RedisClient(RedisSocket socketClient,AddressInfo addressInfo)
        {
            _addressInfo = addressInfo;
            _socketClient = socketClient;
        }

        public byte[] Load(string script){
            
            return SendCommand<byte[]>(RedisCommand.Load(script));
        }

        public byte[] EvalSha(byte[] scriptSha, int noKeys, params string[] keys)
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


        public byte[] LPush(string listId, byte[] value)
        {
            if (listId == null)
                throw new ArgumentNullException("listId");
            if (value == null)
                throw new ArgumentNullException("value");
            return SendCommand<byte[]>(RedisCommand.LPush(listId, value));
        }


        public byte[] Zadd(string set,int score, string value)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[]>(RedisCommand.ZAdd(set,score, value));

        }
        public byte[] Sadd(string set, string value)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[]>(RedisCommand.SAdd(set, value));

        }
        internal byte[][] SMembers(string set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[][]>(RedisCommand.SMembers(set));
        }
        internal byte[][] ZRangeByScore(string set, string inf, string sup)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return SendCommand<byte[][]>(RedisCommand.ZRangeByScore(set, inf, sup));
        }


        public bool Expire(string key, int seconds)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return SendCommand<byte[]>(RedisCommand.Expire(key, seconds)) == Success.ToUtf8Bytes();
        }

        
        private TResult SendCommand<TResult>(IProcessResult<TResult> command)
        {
            var taskConnect = _socketClient.StartConnect(_addressInfo);
            taskConnect.Wait();
            var taskSend = _socketClient.SendAsync<TResult>(command,taskConnect.Result);
            using (var commandProcessor = taskSend.Result)
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
