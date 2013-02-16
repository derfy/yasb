using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface ISubscriptionService
    {
        BusEndPoint[] GetSubscriptionEndPoints(string typeName);
        void AddSubscriberFor(string typeName, BusEndPoint subscriberEndPoint);
        void RemoveSubscriberFor(string typeName, BusEndPoint subscriberEndPoint);
    }
}
