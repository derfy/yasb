using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class SubscriptionServiceConfiguration
    {
        public SubscriptionServiceConfiguration()
        {
           HostName="localhost";
           Port = 27017;
           DatabaseName = "Test";
        }

        public SubscriptionServiceConfiguration WithHostName(string hostName)
        {
            HostName =  hostName;
            return this;
        }
        public SubscriptionServiceConfiguration WithDatabase(string databaseName)
        {
            DatabaseName = databaseName;
            return this;
        }

        internal string HostName { get; private set; }

        internal int Port { get; private set; }

        internal string DatabaseName { get; set; }
    }
}
