using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Tests;
using Yasb.Common.Messaging.Configuration.CommonConnectionConfigurers;
using Yasb.Tests.Scripts;
using Autofac;
using System.Net;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Tests.Messaging.Integration.Redis;
using System.Threading.Tasks;

namespace Yasb.Tests.Messaging.Redis
{
    
    /// <summary>
    /// Summary description for RedisQueueTest
    /// </summary>
    [TestClass]
    public class RedisQueueTest
    {
        private ScriptsCache _scriptsCache;
        private IQueue _queue;
        public RedisQueueTest()
        {
            var configurator = new RedisConfigurator();
            var redisQueueFactory = configurator.ConfigureQueue(q => q.WithEndPoint("vmEndPoint", "queue_test", "consumer")
                                             .ConfigureConnections<FluentIPEndPointConfigurer>(c => c.WithConnection("vmEndPoint", "192.168.127.128")));
            _queue = redisQueueFactory.CreateFromEndPointName("consumer") as RedisQueue;
            _scriptsCache=configurator.ConfigureScriptsCache("192.168.127.128") as ScriptsCache;
            _scriptsCache.EnsureScriptsCached(new string[]{"TestSetup.lua"});
            
        }
        [TestInitialize()]
        public void BeforeTest()
        {
            _scriptsCache.EvalSha("TestSetup.lua", 0);
            var message = new TestMessage();
            MessageEnvelope envelope= new MessageEnvelope(message, _queue.LocalEndPoint, _queue.LocalEndPoint);
            _queue.Push(envelope);
        }
        [Ignore]
        [TestMethod]
        public void ShouldSetTimeoutError()
        {


            MessageEnvelope newEnvelope = null;
            //Get Message
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            //Wait 5 seconds and reget same message
            MessageEnvelope timeoutEnvelope = null;
            _queue.TryDequeue(DateTime.Now.AddSeconds(5), TimeSpan.FromSeconds(5), out timeoutEnvelope);

            Assert.IsNotNull(timeoutEnvelope);
            //Assert It's the same message
            Assert.AreEqual(newEnvelope.Id, timeoutEnvelope.Id);
            //Second retrieval should yield timeout
            Assert.AreEqual("Operation Timed Out", timeoutEnvelope.LastErrorMessage);
        }
       [Ignore]
        [TestMethod]
        public void ShouldRetrieveMessage()
        {


            MessageEnvelope newEnvelope = null;
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
        }
        [Ignore]
       [TestMethod]
       public void ShouldNotRetrieveSameMessageTwice()
       {


           MessageEnvelope newEnvelope = null;
           var t1 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
           var t2 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
           var t3 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
           Task.WaitAll(t1, t2, t3);
           Assert.IsTrue(t1.Result && !t2.Result && !t3.Result || t2.Result && !t1.Result && !t3.Result || t3.Result && !t1.Result && !t2.Result);
       }
        [Ignore]
        [TestMethod]
        public void MarkAsCompleteShouldRemoveMessageFromQueue()
        {


            MessageEnvelope newEnvelope = null;
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            _queue.SetMessageCompleted(newEnvelope.Id);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNull(newEnvelope);
        }



    }
}
