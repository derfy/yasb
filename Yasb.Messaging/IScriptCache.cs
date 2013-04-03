using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Redis.Messaging
{
    public interface IScriptCache
    {
        void Initialize(string[] fileNames, Type type);
        byte[] EvalSha(string fileName, int noKeys, params string[] keys);
    }
}
