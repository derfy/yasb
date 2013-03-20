using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Yasb.Common.Messaging;

namespace Yasb.Redis.Messaging.Client.Interfaces
{
    public interface IConnectionEventArgsPoolFactory
    {
        IRedisSocketAsyncEventArgsPool GetConnectionsFor(AddressInfo addressInfo);
    }
}
