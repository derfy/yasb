using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging.Client.Commands
{
    public class LPushCommand : IRedisCommand<byte[]>
    {
        private string _listId;
        private byte[] _value;
        public LPushCommand(string listId, byte[] value)
        {
            _listId = listId;
            _value = value;
        }


        public byte[] ProcessResponse(ICommandResultProcessor processor)
        {
            return processor.ExpectInt();
        }

        public byte[][] ToBinary
        {
            get { return new byte[3][] { CommandNames.LPush, _listId.ToUtf8Bytes(), _value }; }
        }
    }
}
