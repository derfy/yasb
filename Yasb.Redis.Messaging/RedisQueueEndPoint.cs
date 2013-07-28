using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Net;
using Yasb.Common.Messaging.Connections;

namespace Yasb.Redis.Messaging
{
    public class RedisQueueEndPoint : QueueEndPoint<RedisConnection>
    {
        public RedisQueueEndPoint(RedisConnection connection, string name)
            : base(connection, name)
        {

        }

        public RedisQueueEndPoint(string value):base(value)
        {
            var array = value.Split(':');
            if (array.Length < 3)
                throw new ApplicationException(string.Format("{0} is not a valid EndPoint", value));
            Connection = new RedisConnection(array[0], int.Parse(array[1]));
            Name=array[2];
        }

        public override string Value
        {
            get { 
                return string.Format("{0}:{1}",Connection.Value,Name); 
            }
        }

    }
}
