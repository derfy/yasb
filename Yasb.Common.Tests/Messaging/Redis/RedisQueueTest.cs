using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Configuration;
using Yasb.Common.Tests;

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
            //var resolver = new AutofacConfigurator<TestConnectionConfiguration>().WithEndPointConfigurer(c => c.WithLocalEndPoint("myHost", "queue_test")
            //                                 ).WithMessageHandlersAssembly(typeof(TestMessage).Assembly).Configure();
            //_queue = resolver.GetLocalQueue();
            //_scriptsCache = resolver.ScriptsCacheFor(_queue.LocalEndPoint);
            //_scriptsCache.EnsureScriptCached("TestSetup.lua", typeof(TestMessage));
            
        }
        [TestInitialize()]
        public void BeforeTest()
        {
            _scriptsCache.EvalSha("TestSetup.lua", 0);
            var message = new TestMessage();
            //var envelope = new MessageEnvelope(message,  _queue.LocalEndPoint, _queue.LocalEndPoint);
            //_queue.Push(envelope);
        }
        [Ignore]
        [TestMethod]
        public void ShouldGetMessageOnlyOnce()
        {


            MessageEnvelope newEnvelope = null;
            _queue.TryGetEnvelope(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            _queue.TryGetEnvelope(DateTime.Now.AddSeconds(5), TimeSpan.FromSeconds(5), out newEnvelope);

            Assert.IsNotNull(newEnvelope);
            //Assert.IsNotNull(newEnvelope.LastErrorMessage);
            //queue.TryGetEnvelope(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            //queue.TryGetEnvelope(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            //queue.TryGetEnvelope(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            //queue.TryGetEnvelope(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            //queue.TryGetEnvelope(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            //queue.TryGetEnvelope(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            //Assert.IsNull(newEnvelope);
        }
    }
}
