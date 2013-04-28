using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Tests.Configuration
{
    public class FluentTestConnectionConfigurer : FluentConnectionConfigurer<TestConnection>
    {
        public FluentTestConnectionConfigurer WithConnection(string connectionName, string host = "localhost", int port = 6379)
        {
            base.AddConnection(connectionName, new TestConnection(host, port));
            return this;
        }

    }
}
