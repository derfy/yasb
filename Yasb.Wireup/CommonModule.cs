using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Builder;
using Yasb.Common;
using Yasb.Common.Serialization;
using Newtonsoft.Json;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Wireup
{
    
    public class CommonModule<TConfiguration>: Autofac.Module
    {
        
       
        public CommonModule(TConfiguration configuration, string scope)
        {
            Configuration = configuration;
            Scope = scope;
        }
        protected TConfiguration Configuration { get; private set; }
        protected string Scope { get; private set; }
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            builder.RegisterGeneratedFactory<QueueFactory>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<ISerializer>((componentScope, parameters) =>
            {
                return new Serializer(new JsonConverter[] { new EndPointConverter(), new MessageEnvelopeConverter() });
            }).InstancePerMatchingLifetimeScope(Scope);
            
            
        }
    }
}
