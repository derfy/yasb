using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging.Configuration;
using Yasb.Wireup;
using Yasb.Redis.Messaging.Configuration;
using Yasb.Tests.Common.Serialization;

namespace Yasb.Tests.Common.Messaging.Configuration
{
    /// <summary>
    /// Summary description for ServiceBusConfigurationTest
    /// </summary>
    [TestClass]
    public class RedisServiceBusConfigurationTest
    {
        private ServiceBusConfiguration<TestEndPoint,TestEndPointConfiguration> _sut;
        [TestInitialize]
        public void Setup() {
            _sut = new ServiceBusConfiguration<TestEndPoint, TestEndPointConfiguration>();
        }

        [TestMethod]
        public void ShouldConfigureEndpoint()
        {
            _sut.WithLocalEndPoint("localhost:80:queueName");
            Assert.AreEqual("localhost:80:queueName", _sut.LocalEndPoint.Value);
        }

        [TestMethod]
        public void ShouldConfigureHandlersAssembli()
        {
            _sut.WithMessageHandlersAssembly(typeof(TestMessage).Assembly);
            Assert.AreEqual(typeof(TestMessage).Assembly, _sut.MessageHandlersAssembly);
        }
    }
}
