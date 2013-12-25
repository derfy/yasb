using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Messaging.Configuration
{
    public class QueueConfigurer<TEndPoint> 
    {
        public QueueConfigurer()
        {
            Built = new QueueConfiguration<TEndPoint>();
        }

        public QueueConfigurer<TEndPoint> WithLocalEndPoint(string connectionName, string queueName)
        {
          //  Built.LocalEndPointName = string.Format("{0}@local",queueName);
            return this;
        }



        public QueueConfiguration<TEndPoint> Built { get; private set; }
    }
}
