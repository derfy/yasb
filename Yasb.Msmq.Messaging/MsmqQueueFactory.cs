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

        public override IQueue<MsmqConnection> CreateQueue(MsmqConnection connection, string queueName)
        {
            var endPoint = new MsmqQueueEndPoint(connection, queueName);
            return new MsmqQueue(endPoint, _messageFormatter);
        }

        
       
    }
}
