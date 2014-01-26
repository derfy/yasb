using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IMessageHandler<TMessage>
    {
        void Handle(TMessage msg);
    }
}
