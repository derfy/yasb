using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IMessageHandlerRegistry
    {
        IMessageHandler<TCommand> GetHandlerFor<TCommand>();
    }
}
