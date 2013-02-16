using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Yasb.Common.Messaging;

namespace Yasb.Redis.Messaging
{
    public class RedisEndpoint :BusEndPoint
    {
        public RedisEndpoint(string endPoint):base(endPoint)
        {
      
        }


        public override string Scheme
        {
            get { return "redis"; }
        }
    }
}
