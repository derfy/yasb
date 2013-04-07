using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Redis.Messaging.Configuration
{
    public class RedisEndPointConfiguration : EndPointConfiguration<RedisEndPointConfiguration>
    {

       
        protected override IEndPoint CreateEndPoint(string endPoint)
        {
            return new RedisEndPoint(endPoint);
        }

        public override RedisEndPointConfiguration This
        {
            get { return new RedisEndPointConfiguration(); }
        }
    }
}
