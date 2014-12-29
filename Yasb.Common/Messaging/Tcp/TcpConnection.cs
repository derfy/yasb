using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Yasb.Common.Messaging.Tcp
{
    public abstract class TcpConnection : IDisposable
    {
        private Socket _socket;
        private EndPoint _remoteAddress;
        protected TcpConnection(EndPoint address)
        {
            _remoteAddress = address;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
        }
        
        
        
        public Task<byte[]> SendAsync(byte[][] data)
        {
            var taskCompletionSource = new TaskCompletionSource<byte[]>();
            var args = new SocketAsyncEventArgs() 
            { 
                RemoteEndPoint = _remoteAddress,
                UserToken=taskCompletionSource,
               // BufferList = WriteAllToSendBufferList(data).ToList()
            };
            args.Completed += (object sender, SocketAsyncEventArgs e) => { 
                OnCompleted(sender, e);
                if (e.LastOperation == SocketAsyncOperation.Connect && _socket.Connected) 
                {
                    args.BufferList = WriteAllToSendBufferList(data).ToList();
                    SendAsync(args);
                }
            };
            if (!_socket.Connected)
            {
                ConnectAsync(args);
            }
            else 
            {
                args.BufferList = WriteAllToSendBufferList(data).ToList();
                SendAsync(args); 
            }
            return taskCompletionSource.Task;
        }
        public void Reset(SocketAsyncEventArgs args)
        {
            var buffer = args.Buffer;
            args.SetBuffer(null, 0, 0);
            BufferPool.ReleaseBufferToPool(ref buffer);
            args.UserToken = null;

        }


        

        protected abstract IEnumerable<ArraySegment<byte>> WriteAllToSendBufferList(params byte[][] cmdWithBinaryArgs);



        private  void OnCompleted(object sender, SocketAsyncEventArgs completedArgs)
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
                case SocketAsyncOperation.Disconnect:
                    ProcessDisconnect(completedArgs);
                    break;
            }
        }

       
        
        private void ConnectAsync(SocketAsyncEventArgs args)
        {
            if (!_socket.ConnectAsync(args))
            {
                ProcessConnect(args);
            }
        }

        private void SendAsync(SocketAsyncEventArgs args)
        {
            if (!_socket.SendAsync(args))
            {
                ProcessSend(args);
            }
        }
        private void ReceiveAsync(SocketAsyncEventArgs args)
        {
            if (!_socket.ReceiveAsync(args))
            {
                ProcessReceive(args);
            }
        }
        private void DisconnectAsync(SocketAsyncEventArgs args)
        {
            if (!_socket.DisconnectAsync(args))
            {
                ProcessDisconnect(args);
            }
        }
        private void ProcessConnect(SocketAsyncEventArgs args)
        {
            var tcs = args.UserToken as TaskCompletionSource<byte[]>;
            if (args.SocketError != SocketError.Success)
            {
                ProcessDisconnect(args);
                return;
            }
        }
        private void ProcessSend(SocketAsyncEventArgs args)
        {
            args.BufferList = null;
            args.SetBuffer(BufferPool.GetBuffer(), 0, BufferPool.BufferLength);
            ReceiveAsync(args);
        }
        private void ProcessReceive(SocketAsyncEventArgs args)
        {
            var tcs = args.UserToken as TaskCompletionSource<byte[]>;


            if (args.BytesTransferred == 0 || args.SocketError != SocketError.Success)
            {
                DisconnectAsync(args);
                return;
            }

            var total = args.BytesTransferred + args.Offset;
            if (_socket.Available > 0)
            {
                var buffer = args.Buffer;

                if (total + _socket.Available > buffer.Length)
                {
                    BufferPool.ResizeAndFlushLeft(ref buffer, total + _socket.Available, 0, total);
                }

                args.SetBuffer(buffer, total, buffer.Length - total);

                ReceiveAsync(args);
                return;
            }

            var array = new byte[total];
            Array.Copy(args.Buffer, array, total);
            Reset(args);

            tcs.SetResult(array);
        }

        private void ProcessDisconnect(SocketAsyncEventArgs args)
        {
            var tcs = args.UserToken as TaskCompletionSource<byte[]>;
            Reset(args);
            Dispose();
            tcs.SetCanceled();
        }
        
        

        



        public void Dispose()
        {
        }
    }
}
