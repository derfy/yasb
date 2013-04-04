using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using Yasb.Redis.Messaging.Client;

namespace Yasb.Redis.Messaging
{
    public class ScriptsCache 
    {
        private ConcurrentDictionary<string, Lazy<byte[]>> _internalCache = new ConcurrentDictionary<string, Lazy<byte[]>>();
        private RedisClient _connection;
        private object _lock=new object();
        private volatile bool _initialised=false;
        public ScriptsCache(RedisClient connection)
        {
            _connection = connection;
        }
        

        public byte[] EvalSha(string fileName, int noKeys, params string[] keys)
        {
            var scriptSha = _internalCache[fileName].Value;
            return Connection.EvalSha(scriptSha, noKeys, keys);
        }
        

        public void EnsureScriptCached(string fileName, Type type)
        {
            _internalCache.TryAdd(fileName, new Lazy<byte[]>(() =>
            {
                using (StreamReader reader = new StreamReader(type.Assembly.GetManifestResourceStream(string.Format("{0}.Scripts.{1}", type.Namespace, fileName))))
                {
                    string fileContent = reader.ReadToEnd();
                    return Connection.Load(fileContent);
                }
            }, true));
        }
        internal RedisClient Connection { get { return _connection; } }
        
    }
}
