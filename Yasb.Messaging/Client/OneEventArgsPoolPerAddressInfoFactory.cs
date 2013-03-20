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
    public class OneEventArgsPoolPerAddressInfoFactory : IConnectionEventArgsPoolFactory
    {
        private ConcurrentDictionary<AddressInfo, IRedisSocketAsyncEventArgsPool> _internalDictionary = new ConcurrentDictionary<AddressInfo, IRedisSocketAsyncEventArgsPool>();
        private Func<IRedisSocketAsyncEventArgsPool> _initializer;
        public OneEventArgsPoolPerAddressInfoFactory(Func<IRedisSocketAsyncEventArgsPool> initializer)
        {
            _initializer = initializer;
        }

        public IRedisSocketAsyncEventArgsPool GetConnectionsFor(AddressInfo addressInfo)
        {
            var connectionQueue = _initializer();
            if (_internalDictionary.TryAdd(addressInfo, connectionQueue))
                return connectionQueue;
            return _internalDictionary[addressInfo];
        }

      
    }
}
