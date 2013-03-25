using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IEndPoint
    {
        string Value { get; }
        string Name { get; set; }
    }
}
