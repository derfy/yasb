﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Serialization;

namespace Yasb.Redis.Messaging.Serialization
{
    public class RedisEndPointConverter : EndPointConverter
    {
        protected override IEndPoint CreateEndPoint(string endPoint)
        {
            return new RedisEndPoint(endPoint);
        }
    }
}
