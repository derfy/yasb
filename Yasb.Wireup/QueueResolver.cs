using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Wireup
{
    public class QueueResolver<TConnection>
    {
        private QueueFactory _queueFactory;
        private EndPointConfiguration<TConnection> _endPointConfiguration;
        public QueueResolver(QueueFactory factory, EndPointConfiguration<TConnection> endPointConfiguration)
        {
            _queueFactory = factory;
            _endPointConfiguration = endPointConfiguration;
        }
        public IQueue GetQueueByName(string name)
        {
            var endPoint = _endPointConfiguration.NamedEndPoints.Single(e => e.Name == name);
            return GetQueueForEndPoint(endPoint);
        }
        public IQueue GetQueueForEndPoint(BusEndPoint endPoint)
        {
             return _queueFactory(endPoint);
        }
    }
}
