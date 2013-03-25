using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;

namespace Yasb.Tests.Common.Messaging
{
    public class TestEndPoint : IEndPoint
    {
        private string _endPoint;
        public TestEndPoint(string endPoint)
        {
            _endPoint = endPoint;
        }
        public string Value
        {
            get { return _endPoint; }
        }

        public string Name { get; set; }
    }

}
