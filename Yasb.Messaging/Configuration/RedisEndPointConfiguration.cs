using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Redis.Messaging.Configuration
{
    public class RedisEndPointConfiguration : EndPointConfiguration<RedisEndPoint>
    {

       
        protected override RedisEndPoint CreateEndPoint(string endPoint)
        {
            return new RedisEndPoint(endPoint);
        }
    }
}
