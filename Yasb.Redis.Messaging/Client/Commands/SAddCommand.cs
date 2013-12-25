using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging.Client.Commands
{
    public class SAddCommand : IRedisCommand<byte[]>
    {
        private byte[] _set;
        private byte[] _value;
        public SAddCommand(byte[] set, byte[] value)
        {
            _set = set;
            _value = value;
        }
        public byte[] ProcessResponse(ICommandResultProcessor processor)
        {
            return processor.ExpectInt();
        }
        public byte[][] ToBinary
        {
            get { return new byte[3][] { CommandNames.SAdd, _set, _value }; }
        }
    }
}
