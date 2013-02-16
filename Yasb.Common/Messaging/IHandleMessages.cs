﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Yasb.Common.Messaging
{

    public interface IHandleMessages
    {
   
    }

    public interface IHandleMessages<TMessage> : IHandleMessages where TMessage : IMessage
    {
        void Handle(TMessage message);

    }
}
