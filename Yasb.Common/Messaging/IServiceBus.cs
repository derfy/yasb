using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IServiceBus 
    {
        BusEndPoint LocalEndPoint { get;}
        void Send<TMessage>(BusEndPoint endPoint, TMessage message) where TMessage : IMessage;
        void Publish<TMessage>(TMessage message) where TMessage : IMessage;
        void Subscribe<TMessage>(BusEndPoint endPoint) where TMessage : IMessage;
        void Run();
    }
}
