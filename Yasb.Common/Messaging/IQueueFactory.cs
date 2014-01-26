using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Messaging
{
    public interface IQueueFactory<TEndPointConfiguration> 
    {
        IQueue<TEndPointConfiguration> CreateQueue(TEndPointConfiguration endPoint);
    }

   
}
