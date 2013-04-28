using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class QueueConfiguration<TConnection>
    {
        public EndPointConfiguration EndPointConfiguration { get; internal set; }
        public ConnectionsConfiguration<TConnection> ConnectionConfiguration { get; internal set; }
    }
}
