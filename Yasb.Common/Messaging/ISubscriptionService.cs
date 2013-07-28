using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface ISubscriptionService<TConnection>
    {
        SubscriptionInfo<TConnection>[] GetSubscriptions(string typeName);
        void UnSubscribe(string typeName, string subscriberEndPoint);
    }
}
