﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Messaging.Configuration
{
    public class QueueConfiguration<TEndPoint>
    {

        public TEndPoint LocalEndPoint { get; internal set; }
        
    }
}
