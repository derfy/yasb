using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface ISubscriptionService : IDisposable
    {
        IEndPoint[] GetSubscriptionEndPoints(string typeName);
        void AddSubscriberFor(string typeName, IEndPoint subscriberEndPoint);
        void RemoveSubscriberFor(string typeName, IEndPoint subscriberEndPoint);
    }
}
