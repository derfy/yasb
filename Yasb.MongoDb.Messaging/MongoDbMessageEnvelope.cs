using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;

namespace Yasb.MongoDb.Messaging
{
    public class MongoDbMessageEnvelope : MessageEnvelope
    {
        public long LastCreateOrOpdate { get; set; }
    }
}
