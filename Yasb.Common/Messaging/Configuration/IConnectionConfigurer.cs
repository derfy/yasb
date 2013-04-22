using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IConnectionConfigurer<TConnection> 
    {
        IEnumerable<KeyValuePair<string,TConnection>> Connections { get; }
    }
}
