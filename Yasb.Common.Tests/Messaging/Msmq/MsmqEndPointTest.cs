using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Msmq.Messaging;

namespace Yasb.Tests.Messaging.Msmq
{
    /// <summary>
    /// Summary description for MsmqEndPointTest
    /// </summary>
    [TestClass]
    public class MsmqEndPointTest
    {
        public MsmqEndPointTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

       

        [TestMethod]
        public void ShouldHaveCorrectValue()
        {
            var sut = new MsmqEndPoint("minibuss_publisher1@localhost");
            Assert.AreEqual(@".\private$\minibuss_publisher1", sut.Value);
        }
    }
}
