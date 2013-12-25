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
        void Publish(IMessage message);
        void Subscribe<TMessage>(string topicEndPointName) where TMessage : IMessage;
        void Run();
    }
}
