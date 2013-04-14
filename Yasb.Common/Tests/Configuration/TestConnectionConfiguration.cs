using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Tests.Configuration
{
    public class TestConnectionConfiguration 
    {
        private string host;
        private int port;

        public TestConnectionConfiguration(string host, int port)
        {
             this.host = host;
            this.port = port;
        }
       
    }
}
