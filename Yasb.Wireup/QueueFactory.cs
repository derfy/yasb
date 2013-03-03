using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Autofac.Features.Indexed;

namespace Yasb.Wireup
{
    public class QueueFactory : IQueueFactory
    {
        private IIndex<BusEndPoint, IQueue> _index;
        public QueueFactory(IIndex<BusEndPoint,IQueue> index)
        {
            _index = index;
        }

        public IQueue CreateFromEndPoint(BusEndPoint endPoint)
        {
            return _index[endPoint];
        }
    }
}
