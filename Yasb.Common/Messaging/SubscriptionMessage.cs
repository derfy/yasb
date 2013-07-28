using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class SubscriptionMessage<TConnection> : IMessage
    {
        private List<SubscriptionInfo<TConnection>> _subscriptions = new List<SubscriptionInfo<TConnection>>();

        
        public SubscriptionMessage(string typeName, IEnumerable<SubscriptionInfo<TConnection>> subscriptions)
        {
            TypeName = typeName;
            _subscriptions.AddRange(subscriptions);
        }

        
        public virtual string TypeName { get; private set; }


        public virtual SubscriptionInfo<TConnection>[] Subscriptions { get { return _subscriptions.ToArray(); } }
    }
}
