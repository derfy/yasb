using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yasb.Redis.Messaging.Client.Interfaces
{
    public interface ICommandSender
    {
        ICommandSender Send<TResult>(IProcessResult<TResult> redisCommand);
        Task<ICommandSender> SendAsync<TResult>(IProcessResult<TResult> redisCommand);
        TResult GetLatestResult<TResult>();
    }
}
