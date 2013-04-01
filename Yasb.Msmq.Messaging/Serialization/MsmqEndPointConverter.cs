using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging;

namespace Yasb.Msmq.Messaging.Serialization
{
    public class MsmqEndPointConverter : EndPointConverter
    {
        protected override IEndPoint CreateEndPoint(string endPoint)
        {
            return new MsmqEndPoint(endPoint);
        }
    }
}
