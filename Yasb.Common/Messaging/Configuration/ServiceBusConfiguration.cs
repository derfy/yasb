using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
   
    public class ServiceBusConfiguration<TConnection> 
    {
        private List<BusEndPoint> _endPoints = new List<BusEndPoint>();
        internal ServiceBusConfiguration()
        {

        }
        public Assembly MessageHandlersAssembly { get; internal set; }
        public EndPointConfiguration<TConnection> EndPointConfiguration { get; internal set; }
       
       
    }
}
