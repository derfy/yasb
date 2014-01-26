using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IServiceBus<TEndPoint> 
    {
        TEndPoint LocalEndPoint { get; }
        void Send(string remoteEndPointName, IMessage message);
        void Publish<TMessage>(TMessage message) where TMessage : IMessage;
        void Subscribe(string topicEndPointName);
        void Run();
    }
}
