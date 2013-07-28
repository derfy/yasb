using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Yasb.Common.Messaging.Connections
{
    public class RedisConnection
    {
        public RedisConnection(string host,int port)
        {
            Host = host;
            Port = port;
        }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public string Value { get { return string.Format("{0}:{1}", Host, Port); } }

        public override bool Equals(object obj)
        {
            var other = obj as RedisConnection;
            if (other == null)
                return false;
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
