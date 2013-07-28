using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using Yasb.Common.Extensions;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Yasb.Redis.Messaging.Client
{
    public delegate void ClientDisconnectedEventHandler(RedisSocketAsyncEventArgs sender, EventArgs e);
    public class RedisSocketAsyncEventArgs : SocketAsyncEventArgs,IDisposable
    {
        private Socket _socket;
        public event ClientDisconnectedEventHandler OnClientDisconnected;

        private readonly byte[] endData = new[] { (byte)'\r', (byte)'\n' };

        public RedisSocketAsyncEventArgs()
        {
        }
        internal static RedisSocketAsyncEventArgs CreateNew(EndPoint endPoint)
        {
            return new RedisSocketAsyncEventArgs()
            {
                RemoteEndPoint = endPoint,
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            };
        }


        public void WriteAllToSendBuffer(params byte[][] cmdWithBinaryArgs)
        {
            WriteToSendBuffer(GetCmdBytes('*', cmdWithBinaryArgs.Length));
            foreach (var safeBinaryValue in cmdWithBinaryArgs)
            {
                WriteToSendBuffer(GetCmdBytes('$', safeBinaryValue.Length));
                WriteToSendBuffer(safeBinaryValue);
                WriteToSendBuffer(endData);
            }
        }

        public Task<RedisSocketAsyncEventArgs> StartConnect()
        {
            var tcs = new TaskCompletionSource<RedisSocketAsyncEventArgs>();
            UserToken = tcs;

            if (_socket.Connected)
            {
                tcs.SetResult(this);
            }
            else if (!_socket.ConnectAsync(this))
            {
                ProcessConnect();
            }
            return tcs.Task;
        }


        internal Task<byte[]> SendAsync(byte[][] data)
        {
            var taskCompletionSource = new TaskCompletionSource<byte[]>();
            UserToken = taskCompletionSource;
            SetBuffer(BufferPool.GetBuffer(), 0, BufferPool.BufferLength);
            WriteAllToSendBuffer(data);
            SetBuffer(0, Offset);
            if (!_socket.SendAsync(this))
            {
                ReceiveAsync();
            }
            return taskCompletionSource.Task;
        }


       
        private void WriteToSendBuffer(byte[] cmdBytes)
        {
            var buffer = Buffer;
            var total = Offset + cmdBytes.Length;
            if (cmdBytes.Length > this.Count)
            {
                BufferPool.ResizeAndFlushLeft(ref buffer, total, 0, Offset);
            }
            System.Buffer.BlockCopy(cmdBytes, 0, buffer, Offset, cmdBytes.Length);
            SetBuffer(buffer, total, buffer.Length-total);
        }

        protected override void OnCompleted(SocketAsyncEventArgs e)
        {
            base.OnCompleted(e);
            var completedArgs = e as RedisSocketAsyncEventArgs;
            if (completedArgs != null)
            {
                switch (completedArgs.LastOperation)
                {
                    case SocketAsyncOperation.Connect:
                        ProcessConnect();
                        break;
                    case SocketAsyncOperation.Send:
                        ProcessSend();
                        break;
                    case SocketAsyncOperation.Receive:
                        ProcessReceive();
                        break;
                    case SocketAsyncOperation.Disconnect:
                        ProcessDisconnect();
                        break;
                }

            }
        }

       
        private void ProcessConnect()
        {
            var tcs = UserToken as TaskCompletionSource<RedisSocketAsyncEventArgs>;
            if (SocketError == SocketError.Success)
            {
                tcs.SetResult(this);
                return;
            }
            tcs.SetException(new SocketException((int)SocketError));
        }
        private void ProcessSend()
        {
            SetBuffer(0, BufferPool.BufferLength);
            Array.Clear(Buffer, 0, Buffer.Length);
            ReceiveAsync();
        }
        private void ReceiveAsync()
        {
            if (!_socket.ReceiveAsync(this))
            {
                ProcessReceive();
            }
        }
        private void DisconnectAsync()
        {
            if (!_socket.DisconnectAsync(this))
            {
                ProcessDisconnect();
            }
        }

        private void ProcessDisconnect()
        {
            var tcs = UserToken as TaskCompletionSource<byte[]>;
            Reset();
            Dispose();
            tcs.SetCanceled();
        }
        private void ProcessReceive()
        {
            var tcs = UserToken as TaskCompletionSource<byte[]>;
          
           
            if (BytesTransferred == 0 || SocketError != SocketError.Success)
            {
                DisconnectAsync();
                return;
            }
           
            var total = BytesTransferred + Offset;
            if (_socket.Available > 0)
            {
                var buffer = Buffer;

                if (total + _socket.Available > buffer.Length)
                {
                    BufferPool.ResizeAndFlushLeft(ref buffer, total + _socket.Available, 0, total);
                }

                SetBuffer(buffer, total, buffer.Length - total);
                
                ReceiveAsync();
                return;
            }
           
            var array = new byte[total];
            Array.Copy(Buffer, array, total);
            Reset();
            
            tcs.SetResult(array);
        }

        

        private void Reset()
        {
            var buffer = Buffer;
            SetBuffer(null, 0, 0);
            BufferPool.ReleaseBufferToPool(ref buffer);
            UserToken = null;
           
        }

        private static byte[] GetCmdBytes(char cmdPrefix, int noOfLines)
        {
            var strLines = noOfLines.ToString();
            var strLinesLength = strLines.Length;

            var cmdBytes = new byte[1 + strLinesLength + 2];
            cmdBytes[0] = (byte)cmdPrefix;

            for (var i = 0; i < strLinesLength; i++)
                cmdBytes[i + 1] = (byte)strLines[i];

            cmdBytes[1 + strLinesLength] = 0x0D; // \r
            cmdBytes[2 + strLinesLength] = 0x0A; // \n

            return cmdBytes;
        }

    }
   
}
