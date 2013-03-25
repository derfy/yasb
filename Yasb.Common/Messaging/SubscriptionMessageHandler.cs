using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class SubscriptionMessageHandler<TEndPoint> : IHandleMessages<SubscriptionMessage<TEndPoint>> where TEndPoint : IEndPoint
    {
        private ISubscriptionService _subscriptionService;
        public SubscriptionMessageHandler(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Handle(SubscriptionMessage<TEndPoint> message)
        {
            _subscriptionService.AddSubscriberFor(message.TypeName, message.SubscriberEndPoint);
           
        }

    }
}
