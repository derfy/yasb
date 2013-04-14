using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public delegate IQueue QueueFactory(BusEndPoint endPoint);
}
