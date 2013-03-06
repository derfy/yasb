using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;

namespace Yasb.Tests.Common.Messaging
{
    /// <summary>
    /// Summary description for BusEndPointTest
    /// </summary>
    [TestClass]
    public class BusEndPointTest
    {
       
        [TestMethod]
        public void EndPointShouldBeCorrectlyStringified()
        {
            var busEndPoint = new BusEndPoint();
            busEndPoint.Host = "foo";
            busEndPoint.Port = 80;
            busEndPoint.QueueName = "bar";
            var toString = busEndPoint.ToString();
            Assert.AreEqual("foo:80:bar", toString);
        }

        [TestMethod]
        public void ShouldBeAbleToParse()
        {
            var toString = "foo:80:bar";
            var busEndPoint = BusEndPoint.Parse(toString);
            Assert.AreEqual("foo", busEndPoint.Host);
            Assert.AreEqual(80, busEndPoint.Port);
            Assert.AreEqual("bar", busEndPoint.QueueName);
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ParseShouldThrow()
        {
            var toString = "invalidEndpoint:fgfh";
            var busEndPoint = BusEndPoint.Parse(toString);
        }
    }
}
