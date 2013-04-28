using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Messaging
{
    public abstract class AbstractQueueFactory<TConnection>:IQueueFactory
    {
        private QueueConfiguration<TConnection> _queueConfiguration;
        public AbstractQueueFactory(QueueConfiguration<TConnection> queueConfiguration)
        {
            _queueConfiguration = queueConfiguration;
        }
        public IQueue CreateFromEndPointName(string endPointName)
        {
            var endPointInfo = _queueConfiguration.EndPointConfiguration.GetEndPointInfoByName(endPointName);
            var connection = _queueConfiguration.ConnectionConfiguration.GetConnectionByName(endPointInfo.ConnectionName);
            var queueEndPoint = new EndPointInfo<TConnection>(connection, endPointInfo.QueueName);
            return CreateQueue(connection,endPointInfo.QueueName);
        }

        public abstract IQueue CreateFromEndPointValue(string endPointValue);
        protected abstract IQueue CreateQueue(TConnection connection,string queueName);

    }
}
