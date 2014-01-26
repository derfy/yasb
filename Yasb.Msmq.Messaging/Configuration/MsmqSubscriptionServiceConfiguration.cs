using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Msmq.Messaging.Configuration
{
    public class MsmqSubscriptionServiceConfiguration 
    {
        public MsmqSubscriptionServiceConfiguration()
        {
           HostName="localhost";
           Port = 27017;
           DatabaseName = "Test";
        }

        public MsmqSubscriptionServiceConfiguration WithHostName(string hostName)
        {
            HostName =  hostName;
            return this;
        }
        public MsmqSubscriptionServiceConfiguration WithDatabase(string databaseName)
        {
            DatabaseName = databaseName;
            return this;
        }

        internal string HostName { get; private set; }

        internal int Port { get; private set; }

        internal string DatabaseName { get; set; }
        public MsmqEndPointConfiguration LocalEndPointConfiguration { get; set; }
    }
}
