using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Yasb.Common.Extensions;
using Yasb.Redis.Messaging.Client.Interfaces;

namespace Yasb.Redis.Messaging.Client
{
	public abstract class RedisCommand : IRedisCommand
	{
		internal static IProcessResult<byte[]> Eval(string script, int noKeys, params string[] keys) { return new EvalCommand(script, noKeys, keys); }
		internal static IProcessResult<byte[]> EvalSha(byte[] code, byte[][] args)
		{
			return new EvalShaCommand(code, args);
		}
		internal static IProcessResult<byte[]> SAdd(string set, string value)
		{
			return new SAddCommand(set, value);
		}

		internal static IProcessResult<byte[][]> SMembers(string set)
		{
			return new SMembersCommand(set);
		}
		internal static IProcessResult<byte[]> ZAdd(string set, int score,string value)
		{
			return new ZAddCommand(set,score, value);
		}
		internal static IProcessResult<byte[][]> ZRangeByScore(string set, string inf, string sup)
		{
			return new ZRangeByScoreCommand(set, inf, sup);
		}
		internal static IProcessResult<byte[]> LPush(string listId, byte[] value)
		{
			return new LPushCommand(listId, value);
		}

		internal static IProcessResult<byte[]> HSet(string hashName, string key,byte[] value)
		{
			return new HSetCommand(hashName,key, value);
		}

		
		public static IProcessResult<byte[]> Load(string script) { return new LoadCommand(script); }
		public static IProcessResult<byte[]> SetNX(string key, byte[] value) { return new SetNXCommand(key, value); }

		public static IProcessResult<byte[]> Ttl(string key) { return new TtlCommand(key); }
		public static IProcessResult<byte[]> Get(string key) { return new GetCommand(key); }
		public static IProcessResult<byte[]> Set(string key, byte[] value)
		{
			return new SetCommand(key,value);
		}
		public static IProcessResult<byte[]> Remove(string key) { return new RemoveCommand(key); }
		public static IProcessResult<byte[]> Expire(string key, int seconds) { return new ExpireCommand(key, seconds); }
		 
		public static IProcessResult<byte[]> LPop(string listId) { return new LPopCommand(listId); }
		public static IProcessResult<byte[]> RPop(string listId) { return new RPopCommand(listId); }
		public static IProcessResult<byte[][]> BRPopLPush(string fromListId, string toListId, int timeOutSec) { return new BRPopLPushCommand(fromListId, toListId, timeOutSec); }

		public virtual void SendRequest(RedisSocketAsyncEventArgs e)
		{
			e.PrepareToSend();
			e.WriteAllToSendBuffer(this.ToBinary);
		}



		public abstract byte[][] ToBinary { get; }

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
				get { return new byte[3][] { Commands.SAdd, _set, _value }; }
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
				get { return new byte[2][] { Commands.SMembers, _set }; }
			}
		}
		private class ZAddCommand : RedisCommand, IProcessResult<byte[]>
		{
			private byte[] _set;
			private byte[] _value;
			private byte[] _score;
			public ZAddCommand(string set, int score,string value)
			{
				_score = score.ToUtf8Bytes();
				_set = set.ToUtf8Bytes();
				_value = value.ToUtf8Bytes();
			}
			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectInt();
			}
			public override byte[][] ToBinary
			{
				get { return new byte[4][] { Commands.ZAdd,  _set,_score,_value }; }
			}
		}
		private class ZRangeByScoreCommand : RedisCommand, IProcessResult<byte[][]>
		{
			private byte[] _set;
			private byte[] _inf;
			private byte[] _sup;

			public ZRangeByScoreCommand(string set, string inf, string sup)
			{
				_set = set.ToUtf8Bytes();
				_inf = inf.ToUtf8Bytes();
				_sup = sup.ToUtf8Bytes();
			}

			public byte[][] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectMultiBulkData();
			}
			public override byte[][] ToBinary
			{
				get { return new byte[4][] { Commands.ZRangeByScore, _set, _inf, _sup }; }
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
				get { return new byte[3][] { Commands.Script,Commands.Load, _script.ToUtf8Bytes() }; }
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
				get { return BinaryUtils.MergeCommandWithArgs(Commands.Eval,_keys); }
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
				get { return BinaryUtils.MergeCommandWithArgs(Commands.EvalSha, _sha1,_keys); }
			}
		}
		private class HSetCommand : RedisCommand, IProcessResult<byte[]>
		{
			private byte[] _hashName;
			private byte[] _key;
			private byte[] _value;
			public HSetCommand(string hashName,string key, byte[] value )
			{
				_hashName = hashName.ToUtf8Bytes();
				_key = key.ToUtf8Bytes();
				_value = value;
			}

			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectBulkData();
			}

			public override byte[][] ToBinary
			{
				get { return new byte[3][] { Commands.HSet, _key,_value }; }
			}
		}


		private class SetNXCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string _key;
			private byte[] _value;
			
			public SetNXCommand (string key,byte[] value)
			{
				_key=key;
				_value=value;
			}
			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectInt();
			}
			public  override byte[][]  ToBinary
			{
				get { return new byte[3][] { Commands.SetNx, _key.ToUtf8Bytes(), _value }; }
			}
		}

		private class TtlCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string _key;
			public TtlCommand(string key)
			{
				 _key=key;
			}
			
			public override byte[][]  ToBinary
			{
				get { return new byte[2][] { Commands.Ttl, _key.ToUtf8Bytes() }; }
			}

			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectInt();
			}
		}

		private class GetCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string _key;
			public GetCommand(string key)
			{
				_key=key;
			}


			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectBulkData();
			}

			public override byte[][] ToBinary
			{
				get { return new byte[2][] { Commands.Get, _key.ToUtf8Bytes() }; }
			}
		}
		private class SetCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string _key;
			private byte[] _value;
			
			public SetCommand(string key,byte[] value)
			{
				_key = key;
				_value = value;
			}


			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectSingleLine();
			}

			public override byte[][] ToBinary
			{
				get { return new byte[3][] { Commands.Set, _key.ToUtf8Bytes(), _value }; }
			}
		}
		private class RemoveCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string _key;
			public RemoveCommand(string key)
			{
				_key=key;
			}


			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectInt();
			}

			public override byte[][] ToBinary
			{
				get { return new byte[2][] { Commands.Del, _key.ToUtf8Bytes() };}
			}
		}

		private class ExpireCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string _key;
			public int _seconds;
			public ExpireCommand(string key, int seconds)
			{
				_key=key;
				_seconds=seconds;
			}


			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectInt();
			}

			public override byte[][] ToBinary
			{
				get { return new byte[3][] { Commands.Expire, _key.ToUtf8Bytes(), _seconds.ToUtf8Bytes() }; }
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
				get { return new byte[3][] { Commands.LPush, _listId.ToUtf8Bytes(),_value }; }
			}
		}
		private class RPopCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string _listId;
			public RPopCommand(string listId)
			{
				_listId=listId;
			}


			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectBulkData();
			}

			public override byte[][] ToBinary
			{
				get { return new byte[2][] { Commands.RPop, _listId.ToUtf8Bytes() }; }
			}
		}
		private class LPopCommand : RedisCommand, IProcessResult<byte[]>
		{
			private string _listId;
			public LPopCommand(string listId)
			{
				_listId=listId;
			}


			public byte[] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectBulkData();
			}

			public override byte[][] ToBinary
			{
				get { return new byte[2][] { Commands.LPop, _listId.ToUtf8Bytes() }; }
			}
		}

		private class BRPopLPushCommand : RedisCommand, IProcessResult<byte[][]>
		{
			private string _fromListId;
			private string _toListId;
			private int _timeOutSecs;

			public BRPopLPushCommand(string fromListId, string toListId, int timeOutSecs)
			{
				_fromListId=fromListId;
				_toListId=toListId;
				_timeOutSecs=timeOutSecs;
			}


			public byte[][] ProcessResponse(ICommandResultProcessor processor)
			{
				return processor.ExpectMultiLine();
			}
		
			public  override byte[][]  ToBinary
			{
				get { return new byte[4][] { Commands.BRPopLPush, _fromListId.ToUtf8Bytes(), _toListId.ToUtf8Bytes(), _timeOutSecs.ToUtf8Bytes() }; }
			}
		}


















		
	}

	
}
