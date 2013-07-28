using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Collections.Concurrent;
using Yasb.Common.Messaging;
using System.Net.Sockets;
using System.Net;
using Yasb.Common.Messaging.Connections;
using System.Threading;

namespace Yasb.Redis.Messaging.Client
{
    public class RedisSocketAsyncEventArgsPool : IRedisSocketAsyncEventArgsPool
    {
        private ConcurrentQueue<RedisSocketAsyncEventArgs> _internalQueue = new ConcurrentQueue<RedisSocketAsyncEventArgs>();
        private ManualResetEventSlim _mres = new ManualResetEventSlim(false); 
        private RedisConnection _connection;
        public RedisSocketAsyncEventArgsPool(int size, RedisConnection connection)
        {
            _connection  = connection;

            Initialise(size, Address);
        }

        public EndPoint Address 
        { 
            get 
            {
                var ipAddress = Dns.GetHostAddresses(_connection.Host).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).First();
                return new IPEndPoint(ipAddress, _connection.Port);
            } 
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
