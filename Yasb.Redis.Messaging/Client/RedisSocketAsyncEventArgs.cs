using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using Yasb.Common.Extensions;
using System.Collections.Concurrent;
using System.Net;

namespace Yasb.Redis.Messaging.Client
{
    public class RedisSocketAsyncEventArgs : SocketAsyncEventArgs
    {
        private Socket _socket;
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


        
        public void Reset() {
            var buffer = Buffer;
            Array.Clear(buffer, 0, buffer.Length);
            BufferPool.ReleaseBufferToPool(ref buffer);
            SetBuffer(null, 0, 0);

        }

        internal bool ConnectAsync()
        {
            if (_socket.Connected)
                return false;
            return _socket.ConnectAsync(this);
        }
       
        internal bool SendAsync(params byte[][] data)
        {
            PrepareToSend();
            WriteAllToSendBuffer(data);
            return _socket.SendAsync(this);
        }
        internal bool ReceiveAsync()
        {
            PrepareToReceive();
            return _socket.ReceiveAsync(this);
        }

        
        private void PrepareToSend()
        {
            if (Buffer == null)
            {
                SetBuffer(BufferPool.GetBuffer(), 0, 0);
            }
            else
            {
                SetBuffer(0, 0);
            }
        }
        private void PrepareToReceive()
        {
            SetBuffer(0, Buffer.Length);
        }
        private void WriteToSendBuffer(byte[] cmdBytes)
        {
            int currentIndex = this.Count;
            if (cmdBytes.Length > this.Buffer.Length - currentIndex)
            {
                var buffer = this.Buffer;
                BufferPool.ResizeAndFlushLeft(ref buffer, currentIndex + cmdBytes.Length, 0, currentIndex);

            }
            System.Buffer.BlockCopy(cmdBytes, 0, Buffer, currentIndex, cmdBytes.Length);
            currentIndex += cmdBytes.Length;
            SetBuffer(0, currentIndex);
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
