using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Yasb.Redis.Messaging.Client.Interfaces
{
    public interface IRedisSocketAsyncEventArgsPool
    {
        RedisSocketAsyncEventArgs Dequeue();

        void Enqueue(RedisSocketAsyncEventArgs socketAsyncEventArgs);
        EndPoint Address { get; }
    }
}
