using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Tests.Common.Messaging;
using Yasb.Common.Serialization;

namespace Yasb.Tests.Common.Serialization
{
    public class TestEndPointConverter : EndPointConverter<TestEndPoint>
    {
        protected override TestEndPoint CreateEndPoint(string endPoint)
        {
            throw new NotImplementedException();
        }
    }
}
