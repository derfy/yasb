﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Tests.Common.Messaging.Configuration
{
    /// <summary>
    /// Summary description for ServiceBusConfigurationTest
    /// </summary>
    [TestClass]
    public class ServiceBusConfigurationTest
    {
        private ServiceBusConfiguration _sut;
        [TestInitialize]
        public void Setup() {
            _sut = new ServiceBusConfiguration();
        }

        [TestMethod]
        public void ShouldConfigureEndpoint()
        {
            _sut.WithLocalEndPoint("foo", 80, "queueName");
            Assert.AreEqual("foo:80:queueName", _sut.EndPoint.ToString());
        }

        [TestMethod]
        public void ShouldConfigureHandlersAssembli()
        {
            _sut.WithMessageHandlersAssembly(typeof(FooMessage).Assembly);
            Assert.AreEqual(typeof(FooMessage).Assembly, _sut.MessageHandlersAssembly);
        }
    }
}
