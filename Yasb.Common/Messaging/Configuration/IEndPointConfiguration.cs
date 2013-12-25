using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IEndPointConfiguration<TEndPoint>
    {

        TEndPoint Built { get; }
    }
}
