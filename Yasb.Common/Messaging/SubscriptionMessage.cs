using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class SubscriptionMessage : IMessage
    {
       
        public string TypeName { get; set; }
        public BusEndPoint SubscriberEndPoint { get; set; }
    }
}
