using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Serialization;
using Yasb.Common.Extensions;
using Yasb.Common;

namespace Yasb.Common.Messaging
{
    public class MessagesSender : IMessagesSender
    {
         private Func<BusEndPoint,IQueue> _queueFactory;
         public MessagesSender(Func<BusEndPoint, IQueue> queueFactory)
        {
            _queueFactory = queueFactory;
        }
        public void Send(BusEndPoint endPoint, MessageEnvelope message)
        {
            Guard.NotNull<BusEndPoint>(() => endPoint, endPoint);
            using (var queue = _queueFactory(endPoint)) {
                queue.Push(message);
            }
            
        }

      
    }
}
