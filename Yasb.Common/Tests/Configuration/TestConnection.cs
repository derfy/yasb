using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Tests.Configuration
{
    public class TestConnection 
    {
        private string host;
        private int port;

        public TestConnection(string host, int port)
        {
             this.host = host;
            this.port = port;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}",host,port);
        }
    }
}
