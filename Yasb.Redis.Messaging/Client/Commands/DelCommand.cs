using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging.Client.Commands
{
    public class DelCommand : IRedisCommand<byte[]>
    {
        private byte[] _key;
        public DelCommand(string key)
        {
            _key = key.ToUtf8Bytes();
        }
        public byte[] ProcessResponse(ICommandResultProcessor processor)
        {
            return processor.ExpectInt();
        }
        public byte[][] ToBinary
        {
            get { return new byte[2][] { CommandNames.Del, _key }; }
        }
    }
}
