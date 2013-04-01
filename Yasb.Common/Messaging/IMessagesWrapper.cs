using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IMessagesWrapper
    {
        MessageEnvelope WrapInEnvelope(IMessage message, IEndPoint fromEndPoint);
       
    }
}
