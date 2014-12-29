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
        byte[] EvalSha(string fileName, int noKeys, params byte[][] keys);
        byte[] EvalSha(string scriptName, int noKeys, params string[] keys);
        byte[] Sadd(byte[] set, byte[] value);
        byte[] SRem(string set, string subscriberEndPoint);
        byte[][] SMembers(byte[] set);
        byte[] Del(string key);
    }
}
