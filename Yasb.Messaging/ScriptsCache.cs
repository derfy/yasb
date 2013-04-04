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
        private ConcurrentDictionary<string, byte[]> _internalCache = new ConcurrentDictionary<string, byte[]>();
        private RedisClient _connection;
        private object _lock=new object();
        private volatile bool _initialised=false;
        public ScriptsCache(RedisClient connection)
        {
            _connection = connection;
        }
        

        public byte[] EvalSha(string fileName, int noKeys, params string[] keys)
        {
            var scriptSha = _internalCache[fileName];
            return Connection.EvalSha(scriptSha, noKeys, keys);
        }
        public void EnsureInitialised(string[] fileNames, Type type) { 
            if (!_initialised)
            {
                lock (_lock)
                {
                    if (!_initialised)
                    {
                        Initialize(fileNames, type);
                    }
                }
                    
            }
        }
        internal RedisClient Connection { get { return _connection; } }
        private void Initialize(string[] fileNames, Type type)
        {
            foreach (var fileName in fileNames)
            {
                using (StreamReader reader = new StreamReader(type.Assembly.GetManifestResourceStream(string.Format("{0}.Scripts.{1}", type.Namespace, fileName))))
                {
                    string fileContent = reader.ReadToEnd();
                    _internalCache[fileName] = Connection.Load(fileContent);
                }
            }
            _initialised = true;
        }
    }
}
