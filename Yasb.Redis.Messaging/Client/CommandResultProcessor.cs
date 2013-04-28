using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Threading.Tasks;
using System.IO;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging.Client
{
    public class CommandResultProcessor : ICommandResultProcessor
    {
        private BufferedStream _bStream;
       
        
        internal CommandResultProcessor(byte[] buffer)
        {
             _bStream = new BufferedStream(new MemoryStream(buffer));
     
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
            while (_bStream.Position < _bStream.Length - 1)
            {
                results.Add(RetrieveLineFromBulkData());
            }
            return results.ToArray();
        }

        public void Dispose()
        {
            _bStream.Dispose();
            GC.SuppressFinalize(this); 
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
                var readCount = _bStream.Read(retbuf, offset, count);
                if (readCount <= 0)
                    throw CreateResponseError("Unexpected end of Stream");

                offset += readCount;
                count -= readCount;
            }

            if (_bStream.ReadByte() != '\r' || _bStream.ReadByte() != '\n')
                throw CreateResponseError("Invalid termination");

            return retbuf;


        }
        private byte[] GetSingleLine()
        {
            var bytes = new List<byte>();

            int c;
            while ((c = _bStream.ReadByte()) != -1)
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
            var controlChar = _bStream.ReadByte();//(char)Buffer[currentIndex];
            var result = GetSingleLine();

            if (controlChar == '-')
                throw CreateResponseError(result.FromUtf8Bytes());
            if (controlChar != expectedControlChar)
                throw CreateResponseError(string.Format("{0} was expected but got {1}", expectedControlChar, controlChar));
            return result;

        }

        private byte[] RetrieveLineFromBulkData()
        {
            var controlChar = _bStream.ReadByte();// (char)Buffer[++currentIndex];
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




        private Exception CreateResponseError(string error)
        {
            throw new NotImplementedException();
        }



        
    }
}
