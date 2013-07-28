using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class SubscriptionMessageHandler<TConnection> : IHandleMessages<SubscriptionMessage<TConnection>>,ISubscriptionService<TConnection>
    {
        private ISubscriptionService<TConnection> _subscriptionService;
        public SubscriptionMessageHandler(ISubscriptionService<TConnection> subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Handle(SubscriptionMessage<TConnection> message)
        {
            
        }


        public SubscriptionInfo<TConnection>[] GetSubscriptions(string typeName)
        {
            throw new NotImplementedException();
        }

        public void ProcessSubscriptionMessage(SubscriptionMessage<TConnection> message)
        {
            throw new NotImplementedException();
        }

        public void UnSubscribe(string typeName, string subscriberEndPoint)
        {
            throw new NotImplementedException();
        }
    }
}
