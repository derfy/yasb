using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Yasb.Common.Messaging.Configuration
{
    public abstract class EndPointConfiguration<TConfiguration>
    {
        protected internal abstract IEndPoint CreateEndPoint(string endPoint);

        public TConfiguration WithName(string name)
        {
            Built.Name=name;
            return This;
        }

        public abstract TConfiguration This { get; }

        internal IEndPoint Built { get;  set; }
    }
}
