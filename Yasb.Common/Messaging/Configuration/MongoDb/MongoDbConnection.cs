using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration.MongoDb
{
    public class MongoDbConnection
    {
        public MongoDbConnection(string host,string database, int port)
        {
            Database = database;
            this.Host = host;
            Port = port;
        }

        public string Host { get; private set; }
        public string Database { get; private set; }
        public int Port { get; private set; }
    }
}
