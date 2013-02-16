using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging.Client
{
    public class RedisSocketAsyncEventArgs : SocketAsyncEventArgs {

        private BufferedStream BStream;
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

       
        internal void PrepareToSend()
        {
            if (Buffer == null)
            {
                SetBuffer(BufferPool.GetBuffer(), 0, 0);
            }
            else {
                SetBuffer(0, 0);
            }
        }
        internal void PrepareToReceive()
        {
            BStream = new BufferedStream(new MemoryStream(Buffer));
            SetBuffer(0, Buffer.Length);
        }
        public void Reset() {
            BStream.Dispose();
            var buffer = Buffer;
            BufferPool.ReleaseBufferToPool(ref buffer);
            SetBuffer(null, 0, 0);
        }

       

        

        public byte[] ExpectSingleLine()
        {
            return ExpectLineStartingWith('+');
        }

        public byte[] ExpectInt()
        {
            return ExpectLineStartingWith(':');
        }

        public byte[] ExpectBulkData()
        {
            var firstLine = ExpectLineStartingWith('$').FromUtf8Bytes();
            int dataLength = int.Parse(firstLine);
            return GetSafeSingleLine(dataLength);
        }
        public byte[][] ExpectMultiBulkData()
        {
            var firstLine = ExpectLineStartingWith('*').FromUtf8Bytes();
            int arrayLength = int.Parse(firstLine);
            if (arrayLength < 0)
            {
                return null;
            }
            var results = new byte[arrayLength][];
            for (int i = 0; i < arrayLength; i++)
            {
                results[i] = RetrieveLineFromBulkData();
            }
            
            return results;
        }

        public byte[][] ExpectMultiLine()
        {
            var results = new List<byte[]>();
            while (BStream.Position < this.BytesTransferred-1)
            {
                results.Add(RetrieveLineFromBulkData());
            }
            return results.ToArray();
        }
        
        private byte[] GetSafeSingleLine(int count)
        {
            if (count < 0)
                return null;
            var sb = new StringBuilder();

            var retbuf = new byte[count];

            var offset = 0;
            while (count > 0)
            {
                var readCount = BStream.Read(retbuf, offset, count);
                if (readCount <= 0)
                    throw CreateResponseError("Unexpected end of Stream");

                offset += readCount;
                count -= readCount;
            }

            if (BStream.ReadByte() != '\r' || BStream.ReadByte() != '\n')
                throw CreateResponseError("Invalid termination");

            return retbuf;
           
            
        }
        private byte[] GetSingleLine()
        {
            var bytes = new List<byte>();

            int c;
            while ((c = BStream.ReadByte()) != -1)
            {
                if (c == '\r')
                    continue;
                if (c == '\n')
                    break;
                bytes.Add((byte)c);
            }
            return bytes.ToArray();
           
        }
        private byte[] ExpectLineStartingWith(char expectedControlChar)
        {
            var controlChar = BStream.ReadByte();//(char)Buffer[currentIndex];
            var result = GetSingleLine();

            if (controlChar == '-')
                throw CreateResponseError(result.FromUtf8Bytes());
            if (controlChar != expectedControlChar)
                throw CreateResponseError(string.Format("{0} was expected but got {1}", expectedControlChar, controlChar));
            return result;
            
        }

        private byte[] RetrieveLineFromBulkData()
        {
            var controlChar = BStream.ReadByte();// (char)Buffer[++currentIndex];
            switch (controlChar)
            {
                case '+':
                    return GetSingleLine();
                case ':':
                    return GetSingleLine();
                case '$':
                    var dataLength = GetSingleLine().FromUtf8Bytes();
                    return GetSafeSingleLine(int.Parse(dataLength));
                case '*':
                    var arrayLength = GetSingleLine().FromUtf8Bytes();
                    return null;
                case '-':
                    return GetSingleLine();
                default:
                    throw CreateResponseError(string.Format("Uexpected character {0} found", controlChar));
            }
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



        private Exception CreateResponseError(string p)
        {
            throw new NotImplementedException();
        }
       
    }
   
}
