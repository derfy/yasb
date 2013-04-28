using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class EndPointInfo<TConnection>
    {
        public EndPointInfo(TConnection connection, string queueName)
        {
            Connection = connection;
            QueueName = queueName;
        }
        public TConnection Connection { get; private set; }
        public string QueueName { get; private set; }
    }
}
