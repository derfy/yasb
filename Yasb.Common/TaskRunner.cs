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
        private BlockingCollection<Task> _runningTasks = new BlockingCollection<Task>(MaxRunningTasksNumber);
        private int _runningTasksNumber = 0;
        private object _locker=new object();
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public TaskRunner()
        {
        }

        public void Run(IWorker worker)
        {
            CreateWorkersProducer(_tokenSource.Token,worker);
            CreateWorkersConsumer(_tokenSource.Token);
        }

        private void CreateWorkersConsumer(CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var task in _runningTasks.GetConsumingEnumerable())
                {
                    if (token.IsCancellationRequested)
                        return;
                    task.Start();
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void CreateWorkersProducer(CancellationToken token, IWorker worker)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;
                    lock (_locker)
                    {
                        while (_runningTasksNumber < MaxRunningTasksNumber)
                        {
                            _runningTasks.Add(CreateRunningTask(token,worker));
                            _runningTasksNumber++;
                        }
                        Monitor.Wait(_locker);
                    }

                }
            }, TaskCreationOptions.LongRunning);
        }

        private Task CreateRunningTask(CancellationToken token, IWorker worker)
        {
            Task task = new Task(() =>
            {
                worker.Execute(token);
            }, token, TaskCreationOptions.LongRunning);
            task.ContinueWith(faultedTask =>
            {
                faultedTask.Exception.Handle(ex => 
                {
                    worker.OnException(ex);
                    return true;
                });
               
                lock (_locker)
                {
                    _runningTasksNumber--;
                    Monitor.Pulse(_locker);
                }
            },TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }


        public void Stop()
        {
            _tokenSource.Cancel();
        }
    }
}
