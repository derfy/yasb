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
       
        private Func<RedisSocketAsyncEventArgs> _connectionFactory;
        private EndPoint _endPoint;
        
        public RedisSocketAsyncEventArgsPool(int size,EndPoint endPoint)
        {
            _endPoint = endPoint;
            _connectionFactory = () =>  new RedisSocketAsyncEventArgs()
            {
                RemoteEndPoint = _endPoint,
                AcceptSocket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            };;
            Initialise(size);
        }

        private void Initialise(int size)
        {
            for (int ii = 0; ii < size; ii++)
            {
                _internalQueue.Enqueue(_connectionFactory());
            }
        }

        public RedisSocketAsyncEventArgs Dequeue()
        {
            RedisSocketAsyncEventArgs connectEventArgs=null;
            if (!_internalQueue.TryDequeue(out connectEventArgs))
            {
                connectEventArgs = _connectionFactory();
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
