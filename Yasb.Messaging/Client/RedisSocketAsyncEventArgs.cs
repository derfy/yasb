using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging.Client
{
    public class RedisSocketAsyncEventArgs : SocketAsyncEventArgs
    {

        private readonly byte[] endData = new[] { (byte)'\r', (byte)'\n' };
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


        public void PrepareToSend()
        {
            if (Buffer == null)
            {
                SetBuffer(BufferPool.GetBuffer(), 0, 0);
            }
            else {
                SetBuffer(0, 0);
            }
        }
        public void PrepareToReceive()
        {
             SetBuffer(0, Buffer.Length);
        }
        public void Reset() {
            var buffer = Buffer;
            Array.Clear(buffer, 0, buffer.Length);
            BufferPool.ReleaseBufferToPool(ref buffer);
            SetBuffer(null, 0, 0);
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
