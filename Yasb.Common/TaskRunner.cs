using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Collections.Concurrent;

namespace Yasb.Common
{
    
    public class TaskRunner : ITaskRunner
    {
        private const int MaxRunningTasksNumber=3;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private TaskFactory _taskFactory=new TaskFactory();

        public void Run(Action<CancellationToken> workerAction, Action<Exception> faultedAction = null)
        {
            Parallel.For(0, MaxRunningTasksNumber, index => CreateWorkerTask(workerAction, faultedAction));
               
        }


        public void CreateWorkerTask(Action<CancellationToken> workerAction, Action<Exception> faultedAction = null)
        {
            var token = _tokenSource.Token;
            if (token.IsCancellationRequested)
                return;
            _taskFactory.StartNew(() =>workerAction(token), token)
            .ContinueWith(faultedTask =>
            {
                faultedTask.Exception.Handle(ex =>
                {
                    if (faultedAction != null)
                    {
                        faultedAction(ex);
                    }
                    return true;
                });
            }, TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(t => CreateWorkerTask(workerAction,faultedAction), token,TaskContinuationOptions.ExecuteSynchronously,TaskScheduler.Default);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }
       
    }
}
