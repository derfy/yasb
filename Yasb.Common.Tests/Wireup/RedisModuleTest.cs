using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;
using Yasb.Wireup;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging;
using System.Collections.Concurrent;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Client.Interfaces;
namespace Yasb.Tests.Wireup
{
    /// <summary>
    /// Summary description for RedisModuleTest
    /// </summary>
    [TestClass]
    public class RedisModuleTest
    {
        public RedisModuleTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            var builder = new ContainerBuilder();
            var configuration = new ServiceBusConfiguration();
            Action<IServiceBusConfiguration> configurator = c => {
                c.WithLocalEndPoint(cfg => cfg.WithAddressInfo("127.0.0.1", 80).WithInputQueue("mmgn"));
                c.WithEndPoint("remote", cfg => cfg.WithAddressInfo("192.168.0.1", 80).WithInputQueue("mmgn"));
            };
            configurator(configuration);
            builder.RegisterModule(new RedisModule(configuration));
            var container = builder.Build();
            var addressInfo=new AddressInfo("192.168.0.10",80);
            var factory = container.Resolve<IConnectionEventArgsPoolFactory>();
            var c1=factory.GetConnectionsFor(addressInfo);
            var c2 = factory.GetConnectionsFor(addressInfo);
            var c3 = factory.GetConnectionsFor(configuration.LocalEndPoint.AddressInfo);
        }
    }
}
