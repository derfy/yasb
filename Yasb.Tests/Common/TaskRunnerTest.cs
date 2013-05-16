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
        private TaskRunner _sut;
       

        [TestMethod]
        public void OnlyThreeWorkersShouldBeExecutedAtSameTime()
        {

            CountdownEvent cde = new CountdownEvent(3);
            _sut = new TaskRunner();
            var worker = new Mock<IWorker>();
            worker.Setup(w => w.Execute(It.IsAny<CancellationToken>())).Callback((CancellationToken t) =>
            {
                cde.Signal();
                while (true) { }

            });
            _sut.Run(worker.Object.Execute,worker.Object.OnException);
            cde.Wait();
            worker.Verify(w => w.Execute(It.IsAny<CancellationToken>()), Times.Exactly(3));
           
        }

        [TestMethod]
        public void WorkerShouldHandleException()
        {
            var mre = new ManualResetEventSlim();
            _sut = new TaskRunner();
            var worker = new Mock<IWorker>();
            worker.Setup(w => w.Execute(It.IsAny<CancellationToken>())).Callback((CancellationToken t) =>
            {
                throw new Exception();
            });
            worker.Setup(w => w.OnException(It.IsAny<Exception>())).Callback((Exception t) =>
            {
                Console.WriteLine("jhfg");
                mre.Set();
                while (true) { }
            });
            _sut.CreateWorkerTask(worker.Object.Execute,worker.Object.OnException);
            mre.Wait();
            worker.Verify(w=>w.Execute(It.IsAny<CancellationToken>()), Times.Once());
            worker.Verify(w => w.OnException(It.IsAny<Exception>()), Times.Once());
        }
        [TestMethod]
        public void ShouldBeToStop()
        {
            _sut = new TaskRunner();
            var mre = new ManualResetEventSlim();
            var worker = new Mock<IWorker>();
            worker.Setup(w => w.Execute(It.IsAny<CancellationToken>())).Callback((CancellationToken t) =>
            {
                if (t.IsCancellationRequested)
                    mre.Set();

            });
            _sut.Run(worker.Object.Execute,worker.Object.OnException);
           _sut.Stop();
           mre.Wait();
           Assert.IsTrue(mre.IsSet);
        }
    }
}
