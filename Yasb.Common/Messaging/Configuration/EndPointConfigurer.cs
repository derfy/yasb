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
        
       
        public EndPointConfigurer WithLocalEndPoint(string connectionName, string queueName)
        {
            Built.LocalEndPoint = new BusEndPoint(connectionName, queueName,"local");
            Built.AddNamedEndPoint(Built.LocalEndPoint);
            return this;
        }

        public EndPointConfigurer WithEndPoint(string connectionName, string queueName, string endPointName)
        {
            var endPoint = new BusEndPoint(connectionName, queueName, endPointName);
            Built.AddNamedEndPoint(endPoint);
            return this;
        }

        public EndPointConfiguration Built { get; private set; }
        
        
        
    }
}
