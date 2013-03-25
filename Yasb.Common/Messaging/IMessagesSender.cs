﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IMessagesSender<TEndPoint> 
    {
        void Send(TEndPoint endpoint, MessageEnvelope message);
    }
}
