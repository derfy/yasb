using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Redis.Messaging.Client.Interfaces
{
    public interface IRedisClient
    {
        byte[] LPush(string listId, byte[] value);
        byte[] EvalSha(byte[] scriptSha, int noKeys, params string[] keys);
        byte[] Load(string script);
    }
}
