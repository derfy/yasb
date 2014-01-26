using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;

namespace Yasb.Redis.Messaging.Client.Commands
{
    public class EvalShaCommand : IRedisCommand<byte[]>
    {
        private byte[] _sha1;
        private byte[][] _keys;
        public EvalShaCommand(byte[] sha, params byte[][] args)
        {
            _keys = args;
            _sha1 = sha;
        }

        public byte[] ProcessResponse(ICommandResultProcessor processor)
        {
            return processor.ExpectMultiLine()[0];
        }

        public byte[][] ToBinary
        {
            get { return BinaryUtils.MergeCommandWithArgs(CommandNames.EvalSha, _sha1, _keys); }
        }
    }
}
