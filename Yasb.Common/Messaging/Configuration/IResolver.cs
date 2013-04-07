using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
	public interface IResolver<TResolver>
        where TResolver : IResolver<TResolver> 
    {
        IServiceBus Bus();
        IQueue GetQueueByName(string endPointName);
        IQueue GetLocalQueue();
	}
}
