using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common;
using System.Threading;
using Moq;
using System.Threading.Tasks;

namespace Yasb.Tests.Common
{
    /// <summary>
    /// Summary description for TaskRunnerTest
    /// </summary>
    [TestClass]
    public class TaskRunnerTest
    {
        public class TestResult { }
        private WorkerPool<TestResult> _sut;
       

        [TestMethod]
        public void OnlyThreeWorkersShouldBeExecutedAtSameTime()
        {

            CountdownEvent cde = new CountdownEvent(3);

            var worker = new Mock<IWorker<TestResult>>();
            _sut = new WorkerPool<TestResult>(worker.Object);
            worker.Setup(w => w.Execute()).Callback(() =>
            {
                cde.Signal();
                while (true) { }

            });
            _sut.Run();
            cde.Wait();
            worker.Verify(w => w.Execute(), Times.Exactly(3));
           
        }

        [TestMethod]
        public void WorkerShouldHandleException()
        {
            var mre = new ManualResetEventSlim();
            var worker = new Mock<IWorker<TestResult>>();
            _sut = new WorkerPool<TestResult>(worker.Object);

            worker.Setup(w => w.Execute()).Throws(new Exception());
            worker.Setup(w => w.OnException(It.IsAny<Exception>())).Callback((Exception t) =>
            {
                Console.WriteLine("jhfg");
                mre.Set();
            });
            _sut.Run();
            mre.Wait();
            worker.Verify(w => w.OnException(It.IsAny<Exception>()), Times.Exactly(3));
        }
        [TestMethod]
        public void ShouldBeToStop()
        {
            var mre = new ManualResetEventSlim();
            var worker = new Mock<IWorker<TestResult>>();
            _sut = new WorkerPool<TestResult>(worker.Object);
            worker.Setup(w => w.Execute()).Callback(() =>
            {
                 mre.Set();

            });
            _sut.Run();
           _sut.Stop();
           mre.Wait();
           Assert.IsTrue(mre.IsSet);
        }
    }
}
