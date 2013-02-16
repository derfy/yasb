using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Yasb.Common.Extensions;

namespace Yasb.Redis.Messaging.Client
{
    internal static class BinaryUtils
    {
        internal static byte[][] MergeCommandWithKeysAndValues(byte[] cmd, byte[][] keys, byte[][] values)
        {
            var firstParams = new[] { cmd };
            return MergeCommandWithKeysAndValues(firstParams, keys, values);
        }

        internal static byte[][] MergeCommandWithKeysAndValues(byte[] cmd, byte[] firstArg, byte[][] keys, byte[][] values)
        {
            var firstParams = new[] { cmd, firstArg };
            return MergeCommandWithKeysAndValues(firstParams, keys, values);
        }

        internal static byte[][] MergeCommandWithKeysAndValues(byte[][] firstParams,
            byte[][] keys, byte[][] values)
        {
            if (keys == null || keys.Length == 0)
                throw new ArgumentNullException("keys");
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");
            if (keys.Length != values.Length)
                throw new ArgumentException("The number of values must be equal to the number of keys");

            var keyValueStartIndex = (firstParams != null) ? firstParams.Length : 0;

            var keysAndValuesLength = keys.Length * 2 + keyValueStartIndex;
            var keysAndValues = new byte[keysAndValuesLength][];

            for (var i = 0; i < keyValueStartIndex; i++)
            {
                keysAndValues[i] = firstParams[i];
            }

            var j = 0;
            for (var i = keyValueStartIndex; i < keysAndValuesLength; i += 2)
            {
                keysAndValues[i] = keys[j];
                keysAndValues[i + 1] = values[j];
                j++;
            }
            return keysAndValues;
        }

        internal static byte[][] MergeCommandWithArgs(byte[] cmd, params string[] args)
        {
            var byteArgs = args.ToMultiByteArray();
            return MergeCommandWithArgs(cmd, byteArgs);
        }

        internal static byte[][] MergeCommandWithArgs(byte[] cmd, params byte[][] args)
        {
            var mergedBytes = new byte[1 + args.Length][];
            mergedBytes[0] = cmd;
            for (var i = 0; i < args.Length; i++)
            {
                mergedBytes[i + 1] = args[i];
            }
            return mergedBytes;
        }
        internal static byte[][] MergeCommandWithArgs(byte[][] cmd, byte[] firstArg, params byte[][] args)
        {
            var mergedBytes = new byte[cmd.Length + args.Length+1][];
            for (var i = 0; i < cmd.Length; i++)
            {
                mergedBytes[i] = cmd[i];
            }
            mergedBytes[cmd.Length] = firstArg;
            for (var i = 0; i < args.Length; i++)
            {
                mergedBytes[i + cmd.Length+1] = args[i];
            }
            return mergedBytes;
        }
        internal static byte[][] MergeCommandWithArgs(byte[] cmd, byte[] firstArg, params byte[][] args)
        {
            var mergedBytes = new byte[2 + args.Length][];
            mergedBytes[0] = cmd;
            mergedBytes[1] = firstArg;
            for (var i = 0; i < args.Length; i++)
            {
                mergedBytes[i + 2] = args[i];
            }
            return mergedBytes;
        }

        public static double ParseDouble(byte[] doubleBytes)
        {
            var doubleString = Encoding.UTF8.GetString(doubleBytes);

            double d;
            double.TryParse(doubleString, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out d);

            return d;
        }

        
    }
}
