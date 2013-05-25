using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration.MongoDb
{
    public class MongoDbFluentConnectionConfigurer : FluentConnectionConfigurer<MongoDbConnection>
    {
        public MongoDbFluentConnectionConfigurer WithConnection(string connectionName, string host,string database, int port=27017)
        {
            var connection = new MongoDbConnection(host,database, port);
            AddConnection(connectionName, connection);
            return this;
        }
    }
}
