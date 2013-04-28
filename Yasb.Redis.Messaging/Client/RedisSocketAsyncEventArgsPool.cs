using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Collections.Concurrent;
using Yasb.Common.Messaging;
using System.Net.Sockets;
using System.Net;

namespace Yasb.Redis.Messaging.Client
{
    public class RedisSocketAsyncEventArgsPool : IRedisSocketAsyncEventArgsPool
    {
        private ConcurrentQueue<RedisSocketAsyncEventArgs> _internalQueue = new ConcurrentQueue<RedisSocketAsyncEventArgs>();
       
         
        public RedisSocketAsyncEventArgsPool(int size,EndPoint address)
        {
            Address = address;
           
            Initialise(size,address);
        }

        public EndPoint Address { get; private set; }

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
            if (!_internalQueue.TryDequeue(out connectEventArgs))
            {
                connectEventArgs = RedisSocketAsyncEventArgs.CreateNew(Address);
            }
            return connectEventArgs;
        }

        public int Size { get { return _internalQueue.Count; } }
        
       
       

        public void Enqueue(RedisSocketAsyncEventArgs socketAsyncEventArgs)
        {
           
            _internalQueue.Enqueue(socketAsyncEventArgs);   
            
        }

       

     
       

    }
}
