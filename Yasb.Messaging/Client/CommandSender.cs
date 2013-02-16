using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Threading.Tasks;

namespace Yasb.Redis.Messaging.Client
{
    public class CommandSender : ICommandSender
    {
        private RedisSocket _socketClient;
        private RedisSocketAsyncEventArgs _socketAsyncEventArgs;
        private IRedisCommand _currentCommand;
        public CommandSender(RedisSocket socketClient, RedisSocketAsyncEventArgs args)
        {
            _socketClient = socketClient;
            _socketAsyncEventArgs = args;
        }
        public Task<ICommandSender> SendAsync<TResult>(IProcessResult<TResult> redisCommand)
        {
            _currentCommand = redisCommand;
            return _socketClient.StartSend(_socketAsyncEventArgs, _currentCommand);
        }
        public ICommandSender Send<TResult>(IProcessResult<TResult> redisCommand)
        {
            var task = SendAsync<TResult>(redisCommand);
            task.Wait();
            return this;
        }


        public TResult GetLatestResult<TResult>()
        {
            var command = _currentCommand as IProcessResult<TResult>;
            if (command == null)
                return default(TResult);
            try
            {
                return command.ProcessResponse(_socketAsyncEventArgs);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _socketClient.ReuseConnection(_socketAsyncEventArgs);
            }

        }


    }
}
