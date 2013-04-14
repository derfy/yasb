using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Redis.Messaging;
using Yasb.Redis.Messaging.Configuration;
using Yasb.Common.Tests;
using Yasb.Common.Messaging;

namespace Yasb.Tests.Wireup
{
    /// <summary>
    /// Summary description for RedisResolverTest
    /// </summary>
    [TestClass]
    public class ResolverTest
    {
        private Resolver _sut;
        public ResolverTest()
        {
            _sut = new AutofacConfigurator().ConfigureServiceBus(c => c.WithEndPointConfiguration(cfg => cfg.WithLocalEndPoint("local", "localQueue").WithEndPoint("myHost", "queue_test", "test"))
                .ConfigureConnections<FluentRedisConnectionConfigurer>(conn => conn.WithConnection("local", "127.0.0.1").WithConnection("myHost", "127.0.0.1").WithConnection("myOtherHost","192.198.70.86"))
                                                
                                            .WithMessageHandlersAssembly(typeof(TestMessage).Assembly)).Configure();
        }
       
        [TestMethod]
        public void shouldGetSingleServiceBusInstance()
        {
            var serviceBus1 = _sut.Bus();
            var serviceBus2 = _sut.Bus();
            Assert.AreEqual(serviceBus1, serviceBus2);
        }
        [TestMethod]
        public void shouldBeAbleToGetLocalQueue()
        {
            var localQueue = _sut.GetLocalQueue();
            Assert.AreEqual(new BusEndPoint("local:localQueue"), localQueue.LocalEndPoint);
        }
        [TestMethod]
        public void shouldBeAbleToGetQueueByName()
        {
            var endPointName = "test";
            var queue = _sut.GetQueueByName(endPointName);
            Assert.AreEqual(queue.LocalEndPoint, new BusEndPoint("myHost:queue_test"));
        }

        [TestMethod]
        public void shouldBeAbleToGetRedisClientByEndPoint()
        {
            var endPoint = new BusEndPoint("myHost:queue_test");
            var redisClient = _sut.GetRedisClientByEndPoint(endPoint);
            Assert.IsNotNull(redisClient);
        }

        [TestMethod]
        public void shouldGetSameClientForSameAddress()
        {
            var endPoint1 = new BusEndPoint("myHost:queue_test1");
            var endPoint2 = new BusEndPoint("local:queue_test2");
            var redisClient1 = _sut.GetRedisClientByEndPoint(endPoint1);
            var redisClient2 = _sut.GetRedisClientByEndPoint(endPoint2);
            Assert.AreEqual(redisClient1, redisClient2);
        }
        [TestMethod]
        public void shouldGetDifferentClientsForDifferentAddress()
        {
            var endPoint1 = new BusEndPoint("myHost:queue_test1");
            var endPoint2 = new BusEndPoint("myOtherHost:queue_test1");
            var redisClient1 = _sut.GetRedisClientByEndPoint(endPoint1);
            var redisClient2 = _sut.GetRedisClientByEndPoint(endPoint2);
            Assert.AreNotEqual(redisClient1, redisClient2);
        }
    }
}
