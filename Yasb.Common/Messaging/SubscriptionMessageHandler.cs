using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class SubscriptionMessageHandler : IHandleMessages<SubscriptionMessage>
    {
        private Func<ISubscriptionService> _subscriptionServiceFactory;
        public SubscriptionMessageHandler(Func<ISubscriptionService> subscriptionService)
        {
            _subscriptionServiceFactory = subscriptionService;
        }

        public void Handle(SubscriptionMessage message)
        {
            using (var subscriptionService = _subscriptionServiceFactory())
            {
                subscriptionService.AddSubscriberFor(message.TypeName, message.SubscriberEndPoint);
            }
           
        }

    }
}
