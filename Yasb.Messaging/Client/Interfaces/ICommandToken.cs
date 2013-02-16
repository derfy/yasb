using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yasb.Redis.Messaging.Client.Interfaces
{
    public interface ICommandToken
    {
        IRedisCommand Command { get; }
        void SetException(Exception ex);
        TaskCompletionSource<ICommandSender> TaskCompletionSource { get; }
    }

}
