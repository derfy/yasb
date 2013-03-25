using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Serialization;
using Yasb.Common.Extensions;
using Yasb.Common;

namespace Yasb.Common.Messaging
{
    public class MessagesSender<TEndPoint> : IMessagesSender<TEndPoint> 
    {
        private Func<TEndPoint, IQueue> _queueFactory;
        public MessagesSender(Func<TEndPoint, IQueue> queueFactory)
        {
            _queueFactory = queueFactory;
        }
         public void Send(TEndPoint endPoint, MessageEnvelope message)
        {
            Guard.NotNull<TEndPoint>(() => endPoint, endPoint);
            var queue = _queueFactory(endPoint);
            queue.Push(message);
            
        }

      
    }
}
