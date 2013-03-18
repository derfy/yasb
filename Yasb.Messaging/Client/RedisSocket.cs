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
using Yasb.Common.Messaging;

namespace Yasb.Redis.Messaging.Client
{
   
    
    public class RedisSocket : IDisposable
    {
        private object _disposed = null;
        private int MaxConnectionsNumber;
        private Func<AddressInfo, ConcurrentQueue<RedisSocketAsyncEventArgs>> _connectionEventArgsPoolFactory;
        public RedisSocket(Func<AddressInfo, ConcurrentQueue<RedisSocketAsyncEventArgs>> connectionEventArgsPoolFactory, int maxConnectionsNumber = 5)
        {
            MaxConnectionsNumber = maxConnectionsNumber;
            _connectionEventArgsPoolFactory = connectionEventArgsPoolFactory;
        }



        public Task<RedisSocketAsyncEventArgs> StartConnect(AddressInfo endPoint)
        {
            RedisSocketAsyncEventArgs connectEventArgs = DequeueConnectionEventArgs(endPoint);
           
            var tcs = new TaskCompletionSource<RedisSocketAsyncEventArgs>();
            connectEventArgs.UserToken = tcs;
            bool willRaiseEvent = connectEventArgs.AcceptSocket.ConnectAsync(connectEventArgs);
            if (!willRaiseEvent)
            {
                ProcessConnect(connectEventArgs);
            }
            return tcs.Task;
        }

        private RedisSocketAsyncEventArgs DequeueConnectionEventArgs(AddressInfo endPoint)
        {
            
            var connectionsQueue = _connectionEventArgsPoolFactory(endPoint);
            RedisSocketAsyncEventArgs connectEventArgs;
            if (!connectionsQueue.TryDequeue(out connectEventArgs))
            {
                connectEventArgs=CreateConnectionEventArg(endPoint);

            }
            connectEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            return connectEventArgs;
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




        private void ReuseConnection(RedisSocketAsyncEventArgs socketAsyncEventArgs)
        {
            socketAsyncEventArgs.Reset();
            socketAsyncEventArgs.Completed -= new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            var remoteEndPoint = AddressInfo.CreateForm(socketAsyncEventArgs.RemoteEndPoint);
            var connectionsQueue = _connectionEventArgsPoolFactory(remoteEndPoint);
            connectionsQueue.Enqueue(socketAsyncEventArgs);
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



        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, true) == null)
            {
                //RedisSocketAsyncEventArgs connectionArgs = null;
                //while (_socketAsyncEventArgsPool.TryDequeue(out connectionArgs))
                //{
                //    connectionArgs.Dispose();
                //    connectionArgs = null;
                //}
            }
        }

        private RedisSocketAsyncEventArgs CreateConnectionEventArg(AddressInfo endPoint)
        {
            var connectEventArgs = new RedisSocketAsyncEventArgs()
            {
                RemoteEndPoint = endPoint.ToEndPoint(),
                AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            };
            return connectEventArgs;

        }


    }
}
