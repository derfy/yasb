using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Redis.Messaging.Client.Interfaces
{
    public interface IRedisCommand
    {
        void SendRequest(RedisSocketAsyncEventArgs sendEventArgs);
    }
   
}
