using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Yasb.Common.Messaging.Configuration
{
    public abstract class EndPointConfiguration<TEndPoint> 
        where TEndPoint : IEndPoint
    {
        

        protected internal abstract TEndPoint CreateEndPoint(string endPoint);
       
        public EndPointConfiguration<TEndPoint> WithName(string name)
        {
            Built.Name=name;
            return this;
        }



        internal TEndPoint Built { get;  set; }
    }
}
