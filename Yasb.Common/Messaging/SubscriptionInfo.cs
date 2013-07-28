using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class SubscriptionInfo<TConnection>
    {
        public SubscriptionInfo(QueueEndPoint<TConnection> endPoint, string handler)
        {
            EndPoint = endPoint;
            Handler = handler;
        }

        public QueueEndPoint<TConnection> EndPoint { get; private set; }

        public string Handler { get; private set; }

        public virtual string Value { get { return string.Format("{0}:{1}", EndPoint.Value, Handler); } }
        
    }
}
