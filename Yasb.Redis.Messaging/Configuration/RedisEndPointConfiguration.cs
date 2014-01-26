using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Redis.Messaging.Configuration
{
    public class RedisEndPointConfiguration: IEndPointConfiguration<RedisEndPoint>
    {
        public RedisEndPointConfiguration()
        {
            Port = 6379;
        }
        public RedisEndPointConfiguration(string hostName,string queueName,int port=6379)
        {
            Host = hostName;
            QueueName = queueName;
            Port = port;
        }

        public RedisEndPointConfiguration WithHostName(string hostName)
        {
            Host = hostName;
            return this;
        }

        public object WithQueueName(string queueName)
        {
            QueueName = queueName;
            return this;
        }

        public RedisEndPoint Built
        {
            get { return new RedisEndPoint(Host, QueueName, Port); }
        }


        internal string Host { get; private set; }
        internal string QueueName { get; private set; }


        internal int Port { get; private set; }
    }
}
