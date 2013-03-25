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
    public class ConcurrentQueueRedisSocketEventArgsPool : IRedisSocketAsyncEventArgsPool
    {
        private ConcurrentDictionary<EndPoint, ConcurrentQueue<RedisSocketAsyncEventArgs>> _internalDictionary = new ConcurrentDictionary<EndPoint, ConcurrentQueue<RedisSocketAsyncEventArgs>>();
       
        private int _size;
        private Func<EndPoint, RedisSocketAsyncEventArgs> _connectionFactory;
        public ConcurrentQueueRedisSocketEventArgsPool(int size)
        {
            _connectionFactory = CreateConnectionEventArg;
            _size = size;
        }



        public RedisSocketAsyncEventArgs Dequeue(EndPoint endPoint)
        {
            var currentQueue = PreallocateItems(endPoint);
            RedisSocketAsyncEventArgs connectEventArgs=null;
            if (!currentQueue.TryDequeue(out connectEventArgs))
            {
                connectEventArgs = _connectionFactory(endPoint);
            }
            return connectEventArgs;
        }

        
       
       

        public void Enqueue(RedisSocketAsyncEventArgs socketAsyncEventArgs)
        {
            ConcurrentQueue<RedisSocketAsyncEventArgs> currentQueue = null;
            if (!_internalDictionary.TryGetValue(socketAsyncEventArgs.RemoteEndPoint, out currentQueue))
                throw new ApplicationException("No item was preallocated");
            currentQueue.Enqueue(socketAsyncEventArgs);   
            
        }

        public ConcurrentQueue<RedisSocketAsyncEventArgs> PreallocateItems(EndPoint endPoint)
        {
            ConcurrentQueue<RedisSocketAsyncEventArgs> currentQueue = null;
            if (!_internalDictionary.TryGetValue(endPoint, out currentQueue))
            {
                currentQueue = new ConcurrentQueue<RedisSocketAsyncEventArgs>();
                for (int ii = 0; ii < _size; ii++)
                {
                    currentQueue.Enqueue(_connectionFactory(endPoint));
                }
                _internalDictionary.TryAdd(endPoint, currentQueue);
            }
            return currentQueue;
        }

        public int Size { get { return _internalDictionary.Select(kv => kv.Value).Sum(q => q.Count); } }

       

        private RedisSocketAsyncEventArgs CreateConnectionEventArg(EndPoint endPoint)
        {
            var connectEventArgs = new RedisSocketAsyncEventArgs()
            {
                RemoteEndPoint = endPoint,
                AcceptSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            };
            return connectEventArgs;

        }
       

    }
}
