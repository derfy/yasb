using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using System.Net;

namespace Yasb.Redis.Messaging.Configuration
{
    public class RedisServiceBusConfiguration : ServiceBusConfigurer<EndPoint>
    {
        public RedisServiceBusConfiguration()
        {

        }

    }
}
