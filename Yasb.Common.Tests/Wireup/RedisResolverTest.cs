using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Redis.Messaging;
using Yasb.Redis.Messaging.Configuration;

namespace Yasb.Tests.Wireup
{
    /// <summary>
    /// Summary description for RedisResolverTest
    /// </summary>
    [TestClass]
    public class RedisResolverTest
    {
        private RedisResolver _sut;
        public RedisResolverTest()
        {
            _sut = new AutofacConfigurator().Configure(c => c.WithLocalEndPoint<RedisEndPointConfiguration>("192.168.127.128:6379:localqueue")
                                                .WithEndPoint<RedisEndPointConfiguration>("193.168.127.128:6379:queue_test", conf => conf.WithName("test"))
                                             .WithMessageHandlersAssembly(typeof(TestMessage).Assembly));
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
            Assert.AreEqual(localQueue.LocalEndPoint, new RedisEndPoint("192.168.127.128:6379:localqueue"));
        }
        [TestMethod]
        public void shouldBeAbleToGetQueueByName()
        {
            var endPointName = "test";
            var queue = _sut.GetQueueByName(endPointName);
            Assert.AreEqual(queue.LocalEndPoint, new RedisEndPoint("193.168.127.128:6379:queue_test"));
        }

        [TestMethod]
        public void shouldBeAbleToGetRedisClientByEndPoint()
        {
            var endPoint = new RedisEndPoint("193.168.127.128:6379:queue_test");
            var redisClient = _sut.GetRedisClientByEndPoint(endPoint);
            Assert.IsNotNull(redisClient);
        }

        [TestMethod]
        public void shouldGetSameClientForSameAddress()
        {
            var endPoint1 = new RedisEndPoint("193.168.127.128:6379:queue_test1");
            var endPoint2 = new RedisEndPoint("193.168.127.128:6379:queue_test2");
            var redisClient1 = _sut.GetRedisClientByEndPoint(endPoint1);
            var redisClient2 = _sut.GetRedisClientByEndPoint(endPoint2);
            Assert.AreEqual(redisClient1, redisClient2);
        }
        [TestMethod]
        public void shouldGetDifferentClientsForDifferentAddress()
        {
            var endPoint1 = new RedisEndPoint("192.168.127.128:6379:queue_test");
            var endPoint2 = new RedisEndPoint("193.168.127.128:6379:queue_test");
            var redisClient1 = _sut.GetRedisClientByEndPoint(endPoint1);
            var redisClient2 = _sut.GetRedisClientByEndPoint(endPoint2);
            Assert.AreNotEqual(redisClient1, redisClient2);
        }
    }
}
