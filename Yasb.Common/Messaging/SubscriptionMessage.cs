using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public  class SubscriptionMessage : IMessage
    {
        protected SubscriptionMessage()
        {

        }
        public SubscriptionMessage(string typeName, string subscriberEndPoint)
        {
            TypeName = typeName;
            SubscriberEndPoint = subscriberEndPoint;
        }
        public virtual string TypeName { get; private set; }



        public virtual string SubscriberEndPoint { get; private set; }
    }
}
