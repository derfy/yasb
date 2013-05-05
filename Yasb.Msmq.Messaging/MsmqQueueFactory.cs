using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Serialization;
using System.Messaging;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Msmq.Messaging
{
    public class MsmqQueueFactory : AbstractQueueFactory<MsmqConnection>
    {
        private readonly IMessageFormatter _messageFormatter;
        public MsmqQueueFactory(QueueConfiguration<MsmqConnection> queueConfiguration, IMessageFormatter messageFormatter)
            : base(queueConfiguration)
        {
            _messageFormatter = messageFormatter;
        }

        protected override IQueue CreateQueue(MsmqConnection connection, string queueName)
        {
            var endPointValue = string.Format(@"{0}\{1}$\{2}", connection.Host, connection.IsPrivate ? "Private" : "Public", queueName);
            return CreateFromEndPointValue(endPointValue);
        }

        public override IQueue CreateFromEndPointValue(string endPointValue)
        {
            return new MsmqQueue(endPointValue, _messageFormatter);
        }

       
    }
}
