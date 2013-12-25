using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;

namespace Yasb.Redis.Messaging.Client.Commands
{
    public static class RedisCommandFactory
    {
        public static IRedisCommand<byte[]> Load(string script) { return new LoadCommand(script); }

        public static IRedisCommand<byte[]> Eval(string script, int noKeys, params string[] keys) { return new EvalCommand(script, noKeys, keys); }

        public static IRedisCommand<byte[]> EvalSha(byte[] code, byte[][] args) { return new EvalShaCommand(code, args); }

        public static IRedisCommand<byte[]> SAdd(byte[] set, byte[] value) { return new SAddCommand(set, value); }

        public static IRedisCommand<byte[][]> SMembers(byte[] set) { return new SMembersCommand(set); }

        public static IRedisCommand<byte[]> SRem(string set, string value) { return new SRemCommand(set, value); }

        public static IRedisCommand<byte[]> LPush(string listId, byte[] value) { return new LPushCommand(listId, value); }

        public static IRedisCommand<byte[]> Del(string key) { return new DelCommand(key); }
    }
}
