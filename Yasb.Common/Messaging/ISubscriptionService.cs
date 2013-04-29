using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface ISubscriptionService : IDisposable
    {
        string[] GetSubscriptionEndPoints(string typeName);
        void Subscribe(string typeName, string subscriberEndPoint);
        void UnSubscribe(string typeName, string subscriberEndPoint);
    }
}
