using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yasb.Common.Messaging.Tcp
{
    public abstract class TcpConnectionState : IDisposable
    {
        private const int INACTIVITY_TIMEOUT = 1000 * 5; // 5 secs
        
        private SocketAsyncEventArgs _args;
        private Timer _timer;
        private Socket _socket;
        private bool _isDisconnected = false;
        private int _reserved = 0;

        public TcpConnectionState(EndPoint remoteAddress)
        {
            _args = new SocketAsyncEventArgs() { RemoteEndPoint = remoteAddress,DisconnectReuseSocket=true };
            _args.Completed += (object sender, SocketAsyncEventArgs e) => OnCompleted(sender, e);
            _timer = new Timer(DisconnectAsync, null, int.MaxValue, int.MaxValue);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
     
        public Task<bool> ConnectAsync()
        {
           TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            _args.UserToken = tcs;
            if (!_socket.ConnectAsync(_args))
            {
                ProcessConnect(_socket,_args);
            }
            return tcs.Task;
        }

        public Task<byte[]> SendAsync(byte[][] data)
        {
           TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();
            _args.UserToken = tcs;
            var segments = WriteAllToSendBufferList(data);
            _args.BufferList = segments.ToList();
            if (!_socket.SendAsync(_args))
            {
                ProcessSend(_socket,_args);
            }
            return tcs.Task;
        }

        public bool Reserve() 
        {
            if (0 == Interlocked.Exchange(ref _reserved, 1))
            {
                if (_isDisconnected)
                    return false;
                _timer.Change(int.MaxValue, int.MaxValue);
                return true;
            }
            return false;
          
        }
        public void UnReserve()
        {
            if (_isDisconnected)
                throw new ApplicationException("Object in use is disconnected");
            
            _timer.Change(INACTIVITY_TIMEOUT, int.MaxValue);
            Interlocked.Exchange(ref _reserved, 0);

        }
        public void Dispose()
        {
            ProcessDisconnect(_socket, _args);
        }

        protected abstract IEnumerable<ArraySegment<byte>> WriteAllToSendBufferList(params byte[][] cmdWithBinaryArgs);

        public bool IsBound 
        {
            get
            {

                return _socket.Connected;
            } 
        }

        private void OnCompleted(object sender, SocketAsyncEventArgs completedArgs)
        {
            var socket = sender as Socket;
            switch (completedArgs.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(socket,completedArgs);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(socket,completedArgs);
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive(socket,completedArgs);
                    break;
                case SocketAsyncOperation.Disconnect:
                    ProcessDisconnect(socket, completedArgs);
                    break;
            }
        }
        private void Reset(SocketAsyncEventArgs args)
        {
            var buffer = args.Buffer;
            args.SetBuffer(null, 0, 0);
            BufferPool.ReleaseBufferToPool(ref buffer);

        }

        private void ReceiveAsync(Socket socket, SocketAsyncEventArgs args)
        {
            if (!socket.ReceiveAsync(args))
            {
                ProcessReceive(socket,args);
            }
        }

        private void ProcessReceive(Socket socket,SocketAsyncEventArgs args)
        {
            var taskCompletionSource = args.UserToken as TaskCompletionSource<byte[]>;
            if (args.SocketError != SocketError.Success)
            {
                ProcessDisconnect(socket,args);
                taskCompletionSource.SetException(new SocketException((int)args.SocketError));
                return;
            }
           
            var total = args.BytesTransferred + args.Offset;
            if (socket.Available > 0)
            {
                var buffer = args.Buffer;

                if (total + socket.Available > buffer.Length)
                {
                    BufferPool.ResizeAndFlushLeft(ref buffer, total + socket.Available, 0, total);
                }

                args.SetBuffer(buffer, total, buffer.Length - total);

                ReceiveAsync(socket,args);
                return;
            }

            var array = new byte[total];
            Array.Copy(args.Buffer, array, total);
            Reset(args);
            taskCompletionSource.SetResult(array);
        }

        private void DisconnectAsync(Socket socket, SocketAsyncEventArgs args)
        {
            if (!socket.DisconnectAsync(args))
            {
                ProcessDisconnect(socket,args);
            }
        }

        private void ProcessConnect(Socket socket, SocketAsyncEventArgs args)
        {
            var taskCompletionSource = args.UserToken as TaskCompletionSource<bool>;
            if (args.SocketError != SocketError.Success && args.SocketError != SocketError.IsConnected)
            {
                ProcessDisconnect(socket, args);
                taskCompletionSource.SetException(new SocketException((int)args.SocketError));
                return;
            }
            taskCompletionSource.SetResult(true);
        }
        private void ProcessSend(Socket socket,SocketAsyncEventArgs args)
        {
            args.BufferList = null;
            args.SetBuffer(BufferPool.GetBuffer(), 0, BufferPool.BufferLength);
            ReceiveAsync(socket,args);
        }




        private void DisconnectAsync(object state)
        {
            if (0 == Interlocked.Exchange(ref _reserved, 1))
            {
                _isDisconnected = true;
                if (!_socket.DisconnectAsync(_args))
                {
                    ProcessDisconnect(_socket, _args);
                }
            }
           
           
        }

        private void ProcessDisconnect(Socket socket, SocketAsyncEventArgs args)
        {
            Console.WriteLine("disconnecting ");
            Reset(args);
            _timer.Dispose();
            socket.Close();
        } 
        


        
        private bool IsClientConnected(Socket client)
        {
            bool blockingState = client.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                client.Blocking = false;
                client.Send(tmp, 0, 0);
                Console.WriteLine("Connected!");
                return true;
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK 
                if (e.NativeErrorCode.Equals(10035))
                    Console.WriteLine("Still Connected, but the Send would block");
                else
                {
                    Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);
                }
                return false;
            }
            finally
            {
                client.Blocking = blockingState;
            }

            
        }
       
    }
   
}
