using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Yasb.Common.Messaging.Configuration
{
    public class EndPointConfiguration
    {

       
        public EndPointConfiguration(string endPointName)
        {
            Built = new BusEndPoint(endPointName);
        }

        public EndPointConfiguration WithAddressInfo(string host, int port)
        {
            Built.Port = port;
            Built.Host = host;
            return this;
        }
        public EndPointConfiguration WithInputQueue(string queueName)
        {
            Built.QueueName = queueName;
            return this;
        }


        internal BusEndPoint Built { get; private set; }
    }
}
