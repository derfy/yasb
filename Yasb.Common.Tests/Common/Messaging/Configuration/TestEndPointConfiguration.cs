using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging;

namespace Yasb.Tests.Common.Messaging.Configuration
{
    public class TestEndPointConfiguration : EndPointConfiguration<TestEndPointConfiguration>
    {


        protected override IEndPoint CreateEndPoint(string endPoint)
        {
            return new TestEndPoint(endPoint);
        }



        public override TestEndPointConfiguration This
        {
            get { return new TestEndPointConfiguration(); }
        }
    }
}
