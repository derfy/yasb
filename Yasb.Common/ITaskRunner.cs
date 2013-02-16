using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Yasb.Common
{
    public interface ITaskRunner
    {
        void Run(IWorker worker);
        void Stop();
    }
}
