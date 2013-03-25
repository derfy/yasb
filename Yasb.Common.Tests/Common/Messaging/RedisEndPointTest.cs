using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;

namespace Yasb.Tests.Common.Messaging
{
    /// <summary>
    /// Summary description for BusEndPointTest
    /// </summary>
    [TestClass]
    public class RedisEndPointTest
    {
       
        [TestMethod]
        public void EndPointShouldBeCorrectlyStringified()
        {
            var busEndPoint = new RedisEndPoint("localhost:80:bar");
            var toString = busEndPoint.Value;
            Assert.AreEqual("127.0.0.1:80:bar", toString);
        }

        [TestMethod]
        public void ShouldBeAbleToParse()
        {
            var toString = "localhost:80:bar";
            var busEndPoint = RedisEndPoint.Parse(toString);
            Assert.AreEqual("127.0.0.1:80:bar", busEndPoint.Value);
            Assert.AreEqual("bar", busEndPoint.QueueName);
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ParseShouldThrow()
        {
            var toString = "invalidEndpoint:fgfh";
            var busEndPoint = RedisEndPoint.Parse(toString);
        }
    }
}
