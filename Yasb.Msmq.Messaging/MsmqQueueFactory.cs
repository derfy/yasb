using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Serialization;
using System.Messaging;
using Yasb.Common.Messaging.EndPoints.Msmq;

namespace Yasb.Msmq.Messaging
{
    public class MsmqQueueFactory :IQueueFactory<MsmqEndPoint>
    {
        private readonly IMessageFormatter _messageFormatter;
        public MsmqQueueFactory( IMessageFormatter messageFormatter)
        {
            _messageFormatter = messageFormatter;
        }


        public  IQueue<MsmqEndPoint> CreateQueue(MsmqEndPoint endPoint)
        {
            return new MsmqQueue(endPoint, _messageFormatter);
        }
    }
}
