
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Redis.Messaging;
using Yasb.Common.Tests;
using Yasb.Common.Messaging;
using Autofac;

namespace Yasb.Tests.Wireup
{
    /// <summary>
    /// Summary description for RedisResolverTest
    /// </summary>
    [TestClass]
    public class RegisterOneInstanceForObjectKeyTest
    {
        private IContainer _container;
        [TestInitialize]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterOneInstanceForObjectKey<string, object>((key, c) => new object());
            _container = builder.Build();
        }
        [TestMethod]
        public void shouldRetrieveSameObjectForSameKkey()
        {
            var object1=_container.Resolve<object>(TypedParameter.From<string>("foo"));
            var object2 = _container.Resolve<object>(TypedParameter.From<string>("foo"));
            Assert.AreEqual(object1,object2);
        }
        [TestMethod]
        public void shouldRetrieveDifferentObjectsForDifferentkeys()
        {
            var object1 = _container.Resolve<object>(TypedParameter.From<string>("foo"));
            var object2 = _container.Resolve<object>(TypedParameter.From<string>("bar"));
            Assert.AreNotEqual(object1, object2);
        }
        //private Resolver _sut;
        //public ResolverTest()
        //{
        //    _sut = new AutofacConfigurator().Bus(c => c.WithEndPointConfiguration(cfg => cfg.WithLocalEndPoint("local", "localQueue").WithEndPoint("myHost", "queue_test", "test"))
        //        .ConfigureConnections<FluentIPEndPointConfigurer>(conn => conn.WithConnection("local", "127.0.0.1").WithConnection("myHost", "127.0.0.1").WithConnection("myOtherHost", "192.198.70.86"))
                                                
        //                                    .WithMessageHandlersAssembly(typeof(TestMessage).Assembly));
        //}
       
        //[TestMethod]
        //public void shouldGetSingleServiceBusInstance()
        //{
        //    var serviceBus1 = _sut.Bus();
        //    var serviceBus2 = _sut.Bus();
        //    Assert.AreEqual(serviceBus1, serviceBus2);
        //}
        //[TestMethod]
        //public void shouldBeAbleToGetLocalQueue()
        //{
        //    var localQueue = _sut.GetLocalQueue();
        //  //  Assert.AreEqual(new BusEndPoint("local:localQueue"), localQueue.LocalEndPoint);
        //}
        //[TestMethod]
        //public void shouldBeAbleToGetQueueByName()
        //{
        //    var endPointName = "test";
        //    var queue = _sut.GetQueueByName(endPointName);
        //    //Assert.AreEqual(queue.LocalEndPoint, new BusEndPoint("myHost:queue_test"));
        //}

        //[TestMethod]
        //public void shouldBeAbleToGetRedisClientByEndPoint()
        //{
        //    var endPoint = new BusEndPoint("myHost:queue_test");
        //    var redisClient = _sut.GetRedisClientByEndPoint(endPoint);
        //    Assert.IsNotNull(redisClient);
        //}

        //[TestMethod]
        //public void shouldGetSameClientForSameAddress()
        //{
        //    var endPoint1 = new BusEndPoint("myHost:queue_test1");
        //    var endPoint2 = new BusEndPoint("local:queue_test2");
        //    var redisClient1 = _sut.GetRedisClientByEndPoint(endPoint1);
        //    var redisClient2 = _sut.GetRedisClientByEndPoint(endPoint2);
        //    Assert.AreEqual(redisClient1, redisClient2);
        //}
        //[TestMethod]
        //public void shouldGetDifferentClientsForDifferentAddress()
        //{
        //    var endPoint1 = new BusEndPoint("myHost:queue_test1");
        //    var endPoint2 = new BusEndPoint("myOtherHost:queue_test1");
        //    var redisClient1 = _sut.GetRedisClientByEndPoint(endPoint1);
        //    var redisClient2 = _sut.GetRedisClientByEndPoint(endPoint2);
        //    Assert.AreNotEqual(redisClient1, redisClient2);
        //}
    }
}
