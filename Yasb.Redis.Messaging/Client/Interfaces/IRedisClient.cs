using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Yasb.Redis.Messaging.Client.Interfaces
{
    public interface IRedisClient
    {
        byte[] LPush(string listId, byte[] value);
        byte[] EvalSha(byte[] scriptSha, int noKeys, params string[] keys);
        byte[] Load(string script);
        byte[] Sadd(string set, string value);
        byte[][] SMembers(string set);
        EndPoint Address { get; }
    }
}
