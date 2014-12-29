using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Msmq.Messaging
{
    public class MsmqEndPoint : QueueEndPoint
    {
       
        public MsmqEndPoint(string host,string queueName):base(host,queueName)
        {
            IsPrivate = true;
            
        }

        public bool IsPrivate { get;set; }

        internal string Path { get { return string.Format(@"{0}\{1}$\{2}", Host, IsPrivate ? "private" : "public", QueueName); } }
        public string Value { get { return string.Format(@"{0}:{1}:{2}", Host, IsPrivate ? "private" : "public", QueueName); } }
    }
}
