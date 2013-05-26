using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Yasb.Common.Extensions;
using Yasb.Redis.Messaging.Client.Interfaces;

namespace Yasb.Redis.Messaging.Client
{
	public abstract class RedisCommand 
	{
		internal static IProcessResult<byte[]> Load(string script) { return new LoadCommand(script); }

		internal static IProcessResult<byte[]> Eval(string script, int noKeys, params string[] keys) { return new EvalCommand(script, noKeys, keys); }
		
		internal static IProcessResult<byte[]> EvalSha(byte[] code, byte[][] args){return new EvalShaCommand(code, args);}
		
		internal static IProcessResult<byte[]> SAdd(string set, string value){return new SAddCommand(set, value);}
		
		internal static IProcessResult<byte[][]> SMembers(string set){return new SMembersCommand(set);}
		
		internal static IProcessResult<byte[]> SRem(string set, string value){return new SRemCommand(set,value);}
		
		internal static IProcessResult<byte[]> LPush(string listId, byte[] value){return new LPushCommand(listId, value);}

		internal static IProcessResult<byte[]> Del(string key) { return new DelCommand(key); }

		public abstract byte[][] ToBinary { get; }
		private class DelCommand : RedisCommand, IProcessResult<byte[]>
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
			public override byte[][] ToBinary
			{
				get { return new byte[2][] { CommandNames.Del, _key }; }
			}
		}
		private class SAddCommand : RedisCommand, IProcessResult<byte[]>
		{
			private byte[] _set;
			private byte[] _value;
			public SAddCommand(string set, string value)
			{
			   _set = set.ToUtf8Bytes();
				_value = value.ToUtf8Bytes();
			}
			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectInt();
			}
			public override byte[][] ToBinary
			{
				get { return new byte[3][] { CommandNames.SAdd, _set, _value }; }
			}
		}
		private class SRemCommand : RedisCommand, IProcessResult<byte[]>
		{
			private byte[] _set;
			private byte[] _value;
			public SRemCommand(string set, string value)
			{
				_set = set.ToUtf8Bytes();
				_value = value.ToUtf8Bytes();
			}
			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectInt();
			}
			public override byte[][] ToBinary
			{
				get { return new byte[3][] { CommandNames.SRem, _set, _value }; }
			}
		}
		private class SMembersCommand : RedisCommand, IProcessResult<byte[][]>
		{
			private byte[] _set;

			public SMembersCommand(string set)
			{
				_set = set.ToUtf8Bytes();
			}

			public byte[][] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectMultiBulkData();
			}
			public override byte[][] ToBinary
			{
				get { return new byte[2][] { CommandNames.SMembers, _set }; }
			}
		}
		

		private class LoadCommand : RedisCommand, IProcessResult<byte[]>
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

			public override byte[][] ToBinary
			{
				get { return new byte[3][] { CommandNames.Script,CommandNames.Load, _script.ToUtf8Bytes() }; }
			}
		}

		private class EvalCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string[] _keys;
			public EvalCommand(string script,int noKeys,params string[] keys)
			{
				var list=new List<string>();
				list.AddRange(new string[] { script, noKeys.ToString() });
				list.AddRange(keys);
				_keys = list.ToArray() ;
			}

			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectMultiLine()[0];
			}

			public override byte[][] ToBinary
			{
				get { return BinaryUtils.MergeCommandWithArgs(CommandNames.Eval,_keys); }
			}
		}
		private class EvalShaCommand : RedisCommand, IProcessResult<byte[]>
		{
			private byte[] _sha1;
			private byte[][] _keys;
			public EvalShaCommand(byte[] sha,  params byte[][] args)
			{
				_keys = args;
				_sha1 = sha;
			}

			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectMultiLine()[0];
			}

			public override byte[][] ToBinary
			{
				get { return BinaryUtils.MergeCommandWithArgs(CommandNames.EvalSha, _sha1,_keys); }
			}
		}
		
		private class LPushCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string _listId;
			private byte[] _value;
			public LPushCommand(string listId,byte[] value)
			{
				_listId = listId;
				_value = value;
			}


			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectInt();
			}

			public override byte[][] ToBinary
			{
				get { return new byte[3][] { CommandNames.LPush, _listId.ToUtf8Bytes(),_value }; }
			}
		}
		
		
	}

	
}
