using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Scripts;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Redis.Messaging.Properties;
using System.Globalization;
using System.Resources;
using System.Collections;

namespace Yasb.Redis.Messaging
{
    public class ScriptsCache : IScriptCache
    {
        private ConcurrentDictionary<string, byte[]> _internalCache = new ConcurrentDictionary<string,byte[]>();
        private IRedisClient _connection;
        public ScriptsCache()
        {

        }
        public ScriptsCache(IRedisClient connection)
        {
            _connection = connection;
        }
        

        public virtual byte[] EvalSha(string fileName, int noKeys, params string[] keys)
        {
            var scriptSha = _internalCache[fileName];
            return _connection.EvalSha(scriptSha, noKeys, keys);
        }


        public void EnsureScriptsCached(string[] fileNames,Type probeType=null)
        {
            var type = probeType ?? typeof(RedisScriptsProbe);
            foreach(var fileName in fileNames){
                using (StreamReader reader = new StreamReader(type.Assembly.GetManifestResourceStream(string.Format("{0}.{1}", type.Namespace, fileName))))
                {
                    string fileContent = reader.ReadToEnd();
                    _internalCache.TryAdd(fileName, _connection.Load(fileContent));
                }
            }
        }
        
    }
}
