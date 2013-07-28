using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Msmq.Messaging
{
    public class MsmqQueueEndPoint : QueueEndPoint<MsmqConnection>
    {
        public MsmqQueueEndPoint(MsmqConnection connection, string name)
            : base(connection, name)
        {
        }

        public override string Value { get { return string.Format(@"{0}\{1}$\{2}", Connection.Host, Connection.IsPrivate ? "Private" : "Public", Name); } }


    }
}
