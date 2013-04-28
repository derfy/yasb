using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IServiceBus
    {
        string LocalEndPoint { get; }
        void Send(string endPointName, IMessage message);
        void Send(IQueue endPoint, IMessage message);
        void Publish(IMessage message) ;
        void Subscribe<TMessage>(string endPointName) where TMessage : IMessage;
        void Run();
    }
}
