using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Serialization;
using Yasb.Common.Extensions;
using Yasb.Common;

namespace Yasb.Common.Messaging
{
    //public class MessagesSender<TEndPoint, TEnvelope> : IMessagesSender<TEndPoint, TEnvelope> where TEndPoint : IEndPoint
    //{
    //    private Func<TEndPoint, IQueue<TEnvelope>> _queueFactory;
    //    public MessagesSender(Func<TEndPoint, IQueue<TEnvelope>> queueFactory)
    //    {
    //        _queueFactory = queueFactory;
    //    }
    //    public void Send(TEndPoint endPoint, TEnvelope message)
    //    {
    //        Guard.NotNull<TEndPoint>(() => endPoint, endPoint);
    //        var queue = _queueFactory(endPoint);
    //        queue.Push(message);
            
    //    }

      
    //}
}
