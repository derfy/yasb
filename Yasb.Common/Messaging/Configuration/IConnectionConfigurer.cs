using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IConnectionConfigurer<TConnectionConfiguration> 
    {
        IEnumerable<KeyValuePair<string,TConnectionConfiguration>> Connections { get; }
    }
}
