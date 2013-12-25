using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Redis.Messaging.Client;
using System.Net;
using Moq;
using Yasb.Common.Messaging.Configuration;
using System.Net.Sockets;

namespace Yasb.Tests.Messaging.Redis
{
    /// <summary>
    /// Summary description for ConcurrentQueueRedisSocketEventArgsPoolTest
    /// </summary>
    [TestClass]
    public class RedisSocketAsyncEventArgsPoolTest
    {

        [TestMethod]
        public void PoolShouldHaveSizeItemsAfterCreation()
        {
            var size = 10;
            var endPointMock = CreateEndPoint();
            var sut = new RedisSocketAsyncEventArgsPool(size, endPointMock);
            Assert.AreEqual(size, sut.Size);
        }
        [TestMethod]
        public void ShouldDequeueItem()
        {
            var size = 10;
            var endPointMock = CreateEndPoint();
            var sut = new RedisSocketAsyncEventArgsPool(size, endPointMock);
            var res=sut.Dequeue();
            Assert.AreEqual(size-1, sut.Size);
        }

        
        [TestMethod]
        public void ShouldEnqueueItem()
        {
            var size = 10;
            var endPointMock = CreateEndPoint();
            var sut = new RedisSocketAsyncEventArgsPool(size, endPointMock);
            var item = new RedisSocketAsyncEventArgs();
            sut.Enqueue(item);
            Assert.AreEqual(size + 1, sut.Size);
        }
        private static EndPoint CreateEndPoint()
        {
            var ipAddress = Dns.GetHostAddresses("127.0.0.1").Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).First();
            return new IPEndPoint(ipAddress, 8080);
        }

        //private static Mock<EndPoint> CreateEndPointMock()
        //{
        //    var endPointMock = new Mock<IPEndPoint>();
        //    endPointMock.Setup(e => e.Host).Returns("127.0.0.1");
        //    return endPointMock;
        //}


       
    }
}
