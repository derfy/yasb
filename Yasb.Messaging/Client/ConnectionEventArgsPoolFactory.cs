using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Collections.Concurrent;
using Yasb.Common.Messaging;
using System.Net.Sockets;

namespace Yasb.Redis.Messaging.Client
{
    public class ConnectionEventArgsPoolFactory : IConnectionEventArgsPoolFactory
    {
        private ConcurrentDictionary<AddressInfo, ConcurrentQueue<RedisSocketAsyncEventArgs>> _internalDictionary = new ConcurrentDictionary<AddressInfo, ConcurrentQueue<RedisSocketAsyncEventArgs>>();



        public ConcurrentQueue<RedisSocketAsyncEventArgs> GetConnectionsFor(AddressInfo addressInfo)
        {
            ConcurrentQueue<RedisSocketAsyncEventArgs> connectionQueue = null;
            _internalDictionary.TryGetValue(addressInfo, out connectionQueue);
            if (connectionQueue != null) return connectionQueue;
            _internalDictionary.TryAdd(addressInfo, CreateConnectionQueue(addressInfo));
            return _internalDictionary[addressInfo];
        }

        private static ConcurrentQueue<RedisSocketAsyncEventArgs> CreateConnectionQueue(AddressInfo addressInfo)
        {
            var connectionsQueue = new ConcurrentQueue<RedisSocketAsyncEventArgs>();
            for (int ii = 0; ii < 10; ii++)
            {
                var connectEventArg = new RedisSocketAsyncEventArgs()
                {
                    RemoteEndPoint = addressInfo.ToEndPoint(),
                    AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                };
                connectionsQueue.Enqueue(connectEventArg);
            }
            return connectionsQueue;
        }
    }
}
