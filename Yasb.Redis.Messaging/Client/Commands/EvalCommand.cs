using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Redis.Messaging.Client.Interfaces;

namespace Yasb.Redis.Messaging.Client.Commands
{
    public class EvalCommand : IRedisCommand<byte[]>
    {
        private string[] _keys;
        public EvalCommand(string script, int noKeys, params string[] keys)
        {
            var list = new List<string>();
            list.AddRange(new string[] { script, noKeys.ToString() });
            list.AddRange(keys);
            _keys = list.ToArray();
        }

        public byte[] ProcessResponse(ICommandResultProcessor processor)
        {
            return processor.ExpectMultiLine()[0];
        }

        public byte[][] ToBinary
        {
            get { return BinaryUtils.MergeCommandWithArgs(CommandNames.Eval, _keys); }
        }
    }
}
