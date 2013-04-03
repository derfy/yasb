using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using Yasb.Redis.Messaging.Client;

namespace Yasb.Redis.Messaging
{
    public class ScriptsCache : IScriptCache
    {
        private ConcurrentDictionary<string, byte[]> _internalCache = new ConcurrentDictionary<string, byte[]>();
        private RedisClient _connection;
        public ScriptsCache(RedisClient connection)
        {
            _connection = connection;
        }
        public void Initialize(string[] fileNames, Type type)
        {
            foreach (var fileName in fileNames)
            {
                using (StreamReader reader = new StreamReader(type.Assembly.GetManifestResourceStream(string.Format("{0}.Scripts.{1}", type.Namespace, fileName))))
                {
                    string fileContent = reader.ReadToEnd();
                    _internalCache[fileName] = _connection.Load(fileContent);
                }
            }
        }

        public byte[] EvalSha(string fileName, int noKeys, params string[] keys)
        {
            var scriptSha = _internalCache[fileName];
            return _connection.EvalSha(scriptSha, noKeys, keys);
        }
    }
}
