using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging.Client.Commands
{
    public class LoadCommand : IRedisCommand<byte[]>
    {
        private string _script;
        public LoadCommand(string script)
        {
            _script = script;
        }

        public byte[] ProcessResponse(ICommandResultProcessor processor)
        {
            return processor.ExpectBulkData();
        }

        public byte[][] ToBinary
        {
            get { return new byte[3][] { CommandNames.Script, CommandNames.Load, _script.ToUtf8Bytes() }; }
        }
    }
}
