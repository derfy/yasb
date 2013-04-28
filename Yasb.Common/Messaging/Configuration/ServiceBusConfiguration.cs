using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{

    public class ServiceBusConfiguration<TConnection> : QueueConfiguration<TConnection>
    {  
        public Assembly MessageHandlersAssembly { get; internal set; }
    }
}
