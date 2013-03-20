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
        public ConcurrentQueueRedisSocketEventArgsPool(int size)
        {
            _size = size;
        }



        public RedisSocketAsyncEventArgs Dequeue(EndPoint endPoint)
        {
            var currentQueue = EnsureQueueFor(endPoint);
            RedisSocketAsyncEventArgs connectEventArgs=null;
            if (!currentQueue.TryDequeue(out connectEventArgs))
            {
                connectEventArgs = CreateConnectionEventArg(endPoint);
            }
            return connectEventArgs;
        }

       
       

        public void Enqueue(RedisSocketAsyncEventArgs socketAsyncEventArgs)
        {
            var currentQueue = EnsureQueueFor(socketAsyncEventArgs.RemoteEndPoint);
            currentQueue.Enqueue(socketAsyncEventArgs);
        }

       

        private ConcurrentQueue<RedisSocketAsyncEventArgs> EnsureQueueFor(EndPoint endPoint)
        {
            ConcurrentQueue<RedisSocketAsyncEventArgs> currentQueue = null;
            if(!_internalDictionary.TryGetValue(endPoint,out currentQueue)){
                currentQueue = new ConcurrentQueue<RedisSocketAsyncEventArgs>();
                for (int ii = 0; ii < _size; ii++)
                {
                    currentQueue.Enqueue(CreateConnectionEventArg(endPoint));
                }
                _internalDictionary.TryAdd(endPoint, currentQueue);
            }
            return currentQueue;
        }

        private RedisSocketAsyncEventArgs CreateConnectionEventArg(EndPoint endPoint)
        {
            var connectEventArgs = new RedisSocketAsyncEventArgs()
            {
                RemoteEndPoint = endPoint,
                AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            };
            return connectEventArgs;

        }
       

    }
}
