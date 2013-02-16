using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Net;

namespace Yasb.Redis.Messaging.Client
{
   
    
   
    public class CommandToken : ICommandToken
    {
        private TaskCompletionSource<ICommandSender> _tcs = new TaskCompletionSource<ICommandSender>();
        protected CommandToken()
        {

        }
        public CommandToken(IRedisCommand command)
        {
            this.Command = command;
        }

        public virtual IRedisCommand Command { get; private set; }

        public TaskCompletionSource<ICommandSender> TaskCompletionSource { get { return _tcs; } }



        public void SetException(Exception ex)
        {
            TaskCompletionSource.SetException(ex);
        }
    }
    
    public class RedisSocket
    {
        private ConcurrentQueue<RedisSocketAsyncEventArgs> _connectEventArgsPool;
        private IPEndPoint _ipEndPoint;
        public RedisSocket(IPEndPoint ipEndPoint, int maxConnectionsNumber=10)
        {
            _ipEndPoint = ipEndPoint;
            _connectEventArgsPool = new ConcurrentQueue<RedisSocketAsyncEventArgs>();
          
            for (int i = 0; i < maxConnectionsNumber; i++)
            {
                var connectEventArg = CreateConnectionEventArg();
                _connectEventArgsPool.Enqueue(connectEventArg);
            }
        }

        private RedisSocketAsyncEventArgs CreateConnectionEventArg()
        {
            var connectEventArg = new RedisSocketAsyncEventArgs()
            {
                RemoteEndPoint = _ipEndPoint,
                AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            };
            connectEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            return connectEventArg;
        }

        public Task<ICommandSender> StartConnect()
        {
            RedisSocketAsyncEventArgs connectEventArgs = null;
            while (!_connectEventArgsPool.TryDequeue(out connectEventArgs))
            {
                Thread.Sleep(10);
            }
            var tcs = new TaskCompletionSource<ICommandSender>();
            connectEventArgs.UserToken = tcs;
            bool willRaiseEvent = connectEventArgs.AcceptSocket.ConnectAsync(connectEventArgs);
            if (!willRaiseEvent)
            {
                ProcessConnect(connectEventArgs);
            }
            return tcs.Task;
        }

     
      
        //set the send buffer and post a send op
        internal Task<ICommandSender> StartSend(RedisSocketAsyncEventArgs sendEventArgs, IRedisCommand command)
        {
            var token = new CommandToken(command);
            sendEventArgs.UserToken = token;
            InternalSend(sendEventArgs);
            return token.TaskCompletionSource.Task;
        }

        private void InternalSend(RedisSocketAsyncEventArgs sendEventArgs)
        {
            var token = sendEventArgs.UserToken as ICommandToken;
            token.Command.SendRequest(sendEventArgs);
            bool willRaiseEvent = sendEventArgs.AcceptSocket.SendAsync(sendEventArgs);
            if (!willRaiseEvent)
            {
                ProcessSend(sendEventArgs);
            }
        }

        private void ProcessConnect(RedisSocketAsyncEventArgs connectEventArgs)
        {
            var tcs = connectEventArgs.UserToken as TaskCompletionSource<ICommandSender>;
            if (connectEventArgs.SocketError == SocketError.Success || connectEventArgs.SocketError == SocketError.IsConnected)
            {
                tcs.SetResult(new CommandSender(this, connectEventArgs));
                return;
            }
           
            tcs.SetException(new RedisException("Cannot connect to server"));

        }

        private void ProcessSend(RedisSocketAsyncEventArgs sentEventArgs)
        {
            sentEventArgs.PrepareToReceive();
            bool willRaiseEvent = sentEventArgs.AcceptSocket.ReceiveAsync(sentEventArgs);
            if (!willRaiseEvent)
            {
                ProcessReceive(sentEventArgs);
            }
        }



        private void ProcessReceive(RedisSocketAsyncEventArgs e)
        {
            var token = e.UserToken as ICommandToken;
            if(token!=null)
            {
                token.TaskCompletionSource.SetResult(new CommandSender(this, e));
            }
        }


        internal void ReuseConnection(RedisSocketAsyncEventArgs _socketAsyncEventArgs)
        {
            _socketAsyncEventArgs.Reset();
            _connectEventArgsPool.Enqueue(_socketAsyncEventArgs);
        }
        

       
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            var completedArgs = e as RedisSocketAsyncEventArgs;
            if (completedArgs != null)
            {
                switch (completedArgs.LastOperation)
                {
                    case SocketAsyncOperation.Connect:

                        ProcessConnect(completedArgs);
                        break;
                    case SocketAsyncOperation.Send:

                        ProcessSend(completedArgs);
                        break;
                    case SocketAsyncOperation.Receive:

                        ProcessReceive(completedArgs);
                        break;
                }
               
            }
        }




        
    }
}
