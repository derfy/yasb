using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Scripts;
using Yasb.Redis.Messaging.Client.Interfaces;

namespace Yasb.Redis.Messaging
{
    public class ScriptsCache 
    {
        private ConcurrentDictionary<string, Lazy<byte[]>> _internalCache = new ConcurrentDictionary<string, Lazy<byte[]>>();
        private RedisClient _connection;
        public ScriptsCache()
        {

        }
        public ScriptsCache(RedisClient connection)
        {
            _connection = connection;
        }
        

        public virtual byte[] EvalSha(string fileName, int noKeys, params string[] keys)
        {
            var scriptSha = _internalCache[fileName].Value;
            return _connection.EvalSha(scriptSha, noKeys, keys);
        }


        public void EnsureScriptCached(string fileName)
        {
            _internalCache.TryAdd(fileName, new Lazy<byte[]>(() =>
            {
                var type = typeof(RedisScriptsProbe);
                using (StreamReader reader = new StreamReader(type.Assembly.GetManifestResourceStream(string.Format("{0}.{1}", type.Namespace, fileName))))
                {
                    string fileContent = reader.ReadToEnd();
                    return _connection.Load(fileContent);
                }
            }, true));
        }
        
    }
}
