using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;


namespace Yasb.Redis.Messaging.Client.Interfaces
{
    public interface IRedisCommand<TResult>
    {
        TResult ProcessResponse(ICommandResultProcessor processor);
        byte[][] ToBinary { get; }
    }
}
