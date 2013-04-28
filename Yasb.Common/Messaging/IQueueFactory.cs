using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IQueueFactory { 
        IQueue CreateFromEndPointName(string endPointName);
        IQueue CreateFromEndPointValue(string endPointValue);

       
    }
}
