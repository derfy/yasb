using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.MongoDb.Messaging
{
    public class MongoDbEndPoint : QueueEndPoint
    {
        public MongoDbEndPoint(string host,string queueName)
            :base(host,queueName)
        {
             Port = 27017;
        }

        public string  Database { get;  set; }
        internal int Port { get; set; }

        public  string Value { get { return string.Format("{0}:{1}:{2}", Host, Database, QueueName); } }
    }
}
