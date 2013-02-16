using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Messaging;
using Yasb.Common.Serialization;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging
{
    public class RedisMessagesSender : IMessagesSender
    {
        Func<BusEndPoint, IQueue> _queueFactory;
        public RedisMessagesSender(Func<BusEndPoint,IQueue> queueFactory)
        {
            _queueFactory = queueFactory;
        }
        public void Send(BusEndPoint endPoint, MessageEnvelope message)
        {
            var queue = _queueFactory(endPoint);
            queue.Push(message);
        }

      
    }
}
