using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class SubscriptionMessage<TEndPoint> : IMessage
        where TEndPoint : IEndPoint
    {
       
        public string TypeName { get; set; }
        public TEndPoint SubscriberEndPoint { get; set; }
    }
}
