using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Messaging
{
    public interface IQueueFactory<TEndPoint> 
    {
        IQueue<TEndPoint> CreateQueue(TEndPoint endPoint);
    }

   
}
