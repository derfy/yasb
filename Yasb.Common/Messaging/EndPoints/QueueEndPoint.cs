using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.EndPoints
{
    public  class QueueEndPoint
    {
        public QueueEndPoint()
        {

        }
        public QueueEndPoint(string host, string queueName)
        {
            Host = host;
            QueueName = queueName;
        }
        public string Host { get; private set; }
        public string QueueName { get; private set; }
    }
   
}
