using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging.Client.Commands
{
    public class SMembersCommand : IRedisCommand<byte[][]>
    {
        private byte[] _set;

        public SMembersCommand(byte[] set)
        {
            _set = set;
        }

        public byte[][] ProcessResponse(ICommandResultProcessor processor)
        {
            return processor.ExpectMultiBulkData();
        }
        public byte[][] ToBinary
        {
            get { return new byte[2][] { CommandNames.SMembers, _set }; }
        }
    }
}
