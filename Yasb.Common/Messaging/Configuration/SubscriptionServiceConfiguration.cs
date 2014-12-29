using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class SubscriptionServiceConfiguration
    {
        private int _port = 6379;
        private string _hostName;
        public SubscriptionServiceConfiguration WithHostName(string hostName)
        {
            _hostName = hostName;
            return this;
        }
        public EndPoint ServerAddress
        {
            get
            {
                var ipAddress = Dns.GetHostAddresses(_hostName).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).First();
                return new IPEndPoint(ipAddress, _port);
            }
        }
    }
}
