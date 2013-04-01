using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IEnvelope
    {
        string Id { get; }
        Type ContentType { get; }
        IMessage Message { get;}
    }
}
