using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autofac;
using Yasb.Common.Serialization;

namespace Yasb.Wireup
{
    public class CommonModule<TConfiguration> : ScopedModule<TConfiguration>
    {


        public CommonModule(TConfiguration configuration, string scope)
            : base(configuration, scope)
        {
        }

        protected override void Load(Autofac.ContainerBuilder builder)
        {
         //   builder.RegisterWithScope<JsonConverter>(componentScope => new MessageEnvelopeConverter());
           
            builder.RegisterWithScope<ISerializer>((componentScope, parameters) =>
            {
                return new Serializer(componentScope.Resolve<IEnumerable<JsonConverter>>().ToArray());
            }).InstancePerMatchingLifetimeScope(Scope);


        }
    }
}
