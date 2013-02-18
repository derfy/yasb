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
   
    
    public class RedisSocket 
    {
        private ConcurrentQueue<RedisSocketAsyncEventArgs> _socketAsyncEventArgsPool;
        private IPEndPoint _ipEndPoint;
        private bool _disposed = false;
        public RedisSocket(IPEndPoint ipEndPoint, int maxConnectionsNumber=10)
        {
            _ipEndPoint = ipEndPoint;
            _socketAsyncEventArgsPool = new ConcurrentQueue<RedisSocketAsyncEventArgs>();
          
            for (int i = 0; i < maxConnectionsNumber; i++)
            {
                var connectEventArg = CreateConnectionEventArg();
                _socketAsyncEventArgsPool.Enqueue(connectEventArg);
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

        public Task<RedisSocketAsyncEventArgs> StartConnect()
        {
            RedisSocketAsyncEventArgs connectEventArgs = null;
            while (!_socketAsyncEventArgsPool.TryDequeue(out connectEventArgs))
            {
               Thread.Sleep(10);
            }
            var tcs = new TaskCompletionSource<RedisSocketAsyncEventArgs>();
            connectEventArgs.UserToken = tcs;
            bool willRaiseEvent = connectEventArgs.AcceptSocket.ConnectAsync(connectEventArgs);
            if (!willRaiseEvent)
            {
                ProcessConnect(connectEventArgs);
            }
            return tcs.Task;
        }



        public Task<ICommandResultProcessor> SendAsync<TResult>(IProcessResult<TResult> redisCommand, RedisSocketAsyncEventArgs sendEventArgs)
        {
            var tcs = new TaskCompletionSource<ICommandResultProcessor>();
            sendEventArgs.UserToken = tcs;
            redisCommand.SendRequest(sendEventArgs);
            bool willRaiseEvent = sendEventArgs.AcceptSocket.SendAsync(sendEventArgs);
            if (!willRaiseEvent)
            {
                ProcessSend(sendEventArgs);
            }
            return tcs.Task;
        }


       
        private void ProcessReceive(RedisSocketAsyncEventArgs e)
        {
            var tcs = e.UserToken as TaskCompletionSource<ICommandResultProcessor>;
            if (tcs != null)
            {
                try
                {
                     tcs.SetResult(CommandResultProcessor.CreateFrom(e));
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    ReuseConnection(e);
                }
            }
        }


        private void ProcessConnect(RedisSocketAsyncEventArgs connectEventArgs)
        {
            var tcs = connectEventArgs.UserToken as TaskCompletionSource<RedisSocketAsyncEventArgs>;
            if (connectEventArgs.SocketError == SocketError.Success || connectEventArgs.SocketError == SocketError.IsConnected)
            {
                tcs.SetResult(connectEventArgs);
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




        private void ReuseConnection(RedisSocketAsyncEventArgs _socketAsyncEventArgs)
        {
            _socketAsyncEventArgs.Reset();
            _socketAsyncEventArgsPool.Enqueue(_socketAsyncEventArgs);
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
