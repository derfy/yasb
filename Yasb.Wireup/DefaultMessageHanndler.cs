using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;

namespace Yasb.Wireup
{
    public class DefaultMessageHanndler : IHandleMessages
    {
        public void Handle<TMessage>(TMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
