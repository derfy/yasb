using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Tests.Common.Messaging;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging;

namespace Yasb.Tests.Common.Serialization
{
    public class TestEndPointConverter : EndPointConverter
    {
        protected override IEndPoint CreateEndPoint(string endPoint)
        {
            throw new NotImplementedException();
        }
    }
}
