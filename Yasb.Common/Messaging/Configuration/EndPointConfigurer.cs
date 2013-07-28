using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    
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
