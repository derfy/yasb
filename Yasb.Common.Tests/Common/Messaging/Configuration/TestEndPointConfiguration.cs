using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Tests.Common.Messaging.Configuration
{
    public class TestEndPointConfiguration : EndPointConfiguration<TestEndPoint>
    {


        protected override TestEndPoint CreateEndPoint(string endPoint)
        {
            return new TestEndPoint(endPoint);
        }
    }
}
