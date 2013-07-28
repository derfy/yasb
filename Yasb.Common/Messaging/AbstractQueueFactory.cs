using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Messaging
{
    public abstract class AbstractQueueFactory<TConnection> 
    {
        private QueueConfiguration<TConnection> _queueConfiguration;
        public AbstractQueueFactory(QueueConfiguration<TConnection> queueConfiguration)
        {
            _queueConfiguration = queueConfiguration;
        }
        public IQueue<TConnection> CreateFromEndPointName(string endPointName)
        {
            var endPointInfo = _queueConfiguration.EndPointConfiguration.GetEndPointInfoByName(endPointName);
            var connection = _queueConfiguration.ConnectionConfiguration.GetConnectionByName(endPointInfo.ConnectionName);
            return CreateQueue(connection,endPointInfo.QueueName);
        }

        public abstract IQueue<TConnection> CreateQueue(TConnection connection, string queueName);
    }
}
