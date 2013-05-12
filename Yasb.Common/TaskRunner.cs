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
        private TaskFactory _taskFactory;

        private SemaphoreSlim _resourcePool = new SemaphoreSlim(MaxRunningTasksNumber, MaxRunningTasksNumber);
        public TaskRunner(TaskScheduler taskSheduler)
        {
            _taskFactory = new TaskFactory(taskSheduler);
        }
        public TaskRunner():this(TaskScheduler.Default)
        {

        }
        public void Run(IWorker worker)
        {
            var token = _tokenSource.Token;
            _taskFactory.StartNew(() =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;
                    if(_resourcePool.CurrentCount==0)
                        continue;
                    StartWorker(worker);
                   
                }
            }, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
           
        }


        public void StartWorker(IWorker worker)
        {
            var token = _tokenSource.Token;
            _taskFactory.StartNew(() =>
            {
                try
                {
                    Console.WriteLine("Before  Wait " + _resourcePool.CurrentCount);
                    _resourcePool.Wait(token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                try
                {
                    worker.Execute(token);
                }
                finally
                {

                    _resourcePool.Release();
                    Console.WriteLine("After  Release " + _resourcePool.CurrentCount);
                }
            }).ContinueWith(faultedTask =>
            {
                faultedTask.Exception.Handle(ex =>
                {
                    worker.OnException(ex);
                    return true;
                });
            }, TaskContinuationOptions.OnlyOnFaulted); ;
        }


      

        public void Stop()
        {
            _tokenSource.Cancel();
        }
       
    }
}
