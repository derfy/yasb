using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Msmq.Messaging.Configuration;

namespace Yasb.Msmq.Messaging.Configuration
{
    public class MsmqFluentConnectionConfigurer : FluentConnectionConfigurer<MsmqConnection>
    {
        public MsmqFluentConnectionConfigurer WithConnection(string connectionName, string host,bool isPrivate=true)
        {
            var connection = new MsmqConnection(host,isPrivate);
            AddConnection(connectionName, connection);
            return this;
        }
    }
}
