using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yasb.Common
{
    public interface IWorkerPool
    {
        Task Run();
        void Stop();
    }
}
