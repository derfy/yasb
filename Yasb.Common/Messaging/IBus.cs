﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IBus
    {
        void Send<TMessage>(TMessage command) where TMessage : class;
    }
}