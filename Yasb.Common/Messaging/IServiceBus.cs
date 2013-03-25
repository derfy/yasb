using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IServiceBus<TEndPoint> where TEndPoint : IEndPoint
    {
        TEndPoint LocalEndPoint { get; }
        void Send<TMessage>(string endPointName, TMessage message) where TMessage : IMessage;
        void Send<TMessage>(TEndPoint endPoint, TMessage message) where TMessage : IMessage;
        void Publish<TMessage>(TMessage message) where TMessage : IMessage;
        void Subscribe<TMessage>(string endPointName) where TMessage : IMessage;
        void Run();
    }
}
