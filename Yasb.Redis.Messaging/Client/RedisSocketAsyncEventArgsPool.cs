using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Collections.Concurrent;
using Yasb.Common.Messaging;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Yasb.Redis.Messaging.Client
{
    public class RedisSocketAsyncEventArgsPool : IRedisSocketAsyncEventArgsPool
    {
        private ConcurrentQueue<RedisSocketAsyncEventArgs> _internalQueue = new ConcurrentQueue<RedisSocketAsyncEventArgs>();
        private ManualResetEventSlim _mres = new ManualResetEventSlim(false);
        public RedisSocketAsyncEventArgsPool(int size, EndPoint address)
        {
            Address  = address;

            Initialise(size, Address);
        }

        public EndPoint Address 
        { 
            get;private set;
           
        }

        private void Initialise(int size, EndPoint endPoint)
        {
            for (int ii = 0; ii < size; ii++)
            {
                _internalQueue.Enqueue(RedisSocketAsyncEventArgs.CreateNew(endPoint));
            }
        }

        public RedisSocketAsyncEventArgs Dequeue()
        {
           
            RedisSocketAsyncEventArgs connectEventArgs=null;
            while (!_internalQueue.TryDequeue(out connectEventArgs))
            {
                _mres.Wait();
                _mres.Reset();
            }
            return connectEventArgs;
        }

        public int Size { get { return _internalQueue.Count; } }
        
       
       

        public void Enqueue(RedisSocketAsyncEventArgs socketAsyncEventArgs)
        {
           
            _internalQueue.Enqueue(socketAsyncEventArgs);
            _mres.Set();
        }

       

     
       

    }
}
