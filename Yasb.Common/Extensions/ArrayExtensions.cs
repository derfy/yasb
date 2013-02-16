using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Yasb.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static byte[][] ToMultiByteArray(this string[] args)
        {
            var byteArgs = new byte[args.Length][];
            for (var i = 0; i < args.Length; ++i)
                byteArgs[i] = args[i].ToUtf8Bytes();
            return byteArgs;
        }
        
    }
}
