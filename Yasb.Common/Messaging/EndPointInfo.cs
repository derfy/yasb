using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class EndPointInfo
    {
        public EndPointInfo(string connectionName, string queueName, string name)
        {
            ConnectionName = connectionName;
            QueueName = queueName;
            Name = name;
        }
        public string ConnectionName { get; private set; }
        public string QueueName { get; private set; }
        public string Name { get; private set; }
    }
}
