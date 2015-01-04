using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Yasb.Common.Messaging.Tcp
{
    public interface ITcpConnectionsPool<TConnection> where TConnection : TcpConnectionState 
    {
        TConnection Dequeue();

        void Enqueue(TConnection connection);
    }
}
