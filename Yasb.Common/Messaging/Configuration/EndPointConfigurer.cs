using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public class QueueEndPointInfo
    {
        public QueueEndPointInfo(string connectionName,string queueName,string name)
        {
            ConnectionName = connectionName;
            QueueName = queueName;
            Name = name;
        }
        public string ConnectionName { get; private set; }
        public string QueueName { get; private set; }
        public string Name { get; private set; } 
    }
    public class EndPointConfigurer
    {
        
        public EndPointConfigurer()
        {
            Built = new EndPointConfiguration();
        }

        public EndPointConfigurer WithEndPoint(string connectionName, string queueName, string endPointName)
        {
            Built.AddNamedEndPoint(connectionName,queueName, endPointName);
            return this;
        }


        public EndPointConfiguration Built { get; private set; }
        
    }
    public class ServiceBusEndPointConfigurer : EndPointConfigurer
    {
       
        public ServiceBusEndPointConfigurer WithLocalEndPoint(string connectionName, string queueName)
        {
            WithEndPoint(connectionName,queueName,"local");
            return this;
        }
    }
}
