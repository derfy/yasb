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

        public override int GetHashCode()
        {
            return _endPoint.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var endPoint = obj as TestEndPoint;
            if (endPoint == null) return false;
            return endPoint._endPoint.Equals(_endPoint);
        }
    }

}
