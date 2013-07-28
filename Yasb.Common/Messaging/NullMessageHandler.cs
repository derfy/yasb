using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    internal class NullMessageHandler<TMessage> : IMessageHandler<TMessage> where TMessage: IMessage
    {
        public void Handle(TMessage msg)
        {
        }
    }
}
