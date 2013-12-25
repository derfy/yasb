using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface ISubscriptionService<TEndPoint>
    {
        TEndPoint[] GetSubscriptionEndPoints();
        void UnSubscribe(string topicName, TEndPoint subscriberEndPoint);

        void SubscribeTo(TEndPoint topicEndPoint);
    }
}
