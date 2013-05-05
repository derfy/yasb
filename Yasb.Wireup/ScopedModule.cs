using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common;

namespace Yasb.Wireup
{
    
    public class ScopedModule<TConfiguration>: Autofac.Module
    {
        public ScopedModule(TConfiguration configuration, string scope)
        {
            Configuration = configuration;
            Scope = scope;
        }
        protected TConfiguration Configuration { get; private set; }
        protected string Scope { get; private set; }
       
    }
    
}
