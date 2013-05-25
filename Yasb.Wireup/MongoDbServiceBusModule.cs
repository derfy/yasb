using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration.MongoDb;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Wireup
{
    public class MongoDbServiceBusModule : ServiceBusModule<MongoDbConnection> 
    {
        public MongoDbServiceBusModule(ServiceBusConfiguration<MongoDbConnection> configuration)
            : base(configuration)
        {
        }
    }
}
