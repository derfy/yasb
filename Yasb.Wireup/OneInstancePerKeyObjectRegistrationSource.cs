using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using Autofac.Builder;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Net;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Linq.Expressions;
using Autofac.Features.GeneratedFactories;

namespace Yasb.Wireup
{
    public class OneInstancePerKeyObjectRegistrationSource<TKeyObject, TInstance> : IRegistrationSource where TKeyObject : class
    {
        private ConcurrentDictionary<TKeyObject, TInstance> _internalDictionary = new ConcurrentDictionary<TKeyObject, TInstance>();
        private Func<TKeyObject, IComponentContext, TInstance> _instanceFactory;
        public OneInstancePerKeyObjectRegistrationSource(Func<TKeyObject, IComponentContext,TInstance> instanceFactory)
        {
            _instanceFactory = instanceFactory;
        }
        public bool IsAdapterForIndividualComponents
        {
            get { return false; }
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (registrationAccessor == null) throw new ArgumentNullException("registrationAccessor");

            var ts = service as IServiceWithType;
            if (ts != null && ts.ServiceType == typeof(TInstance))
            {
                var rb = RegistrationBuilder.ForDelegate(ts.ServiceType, (c, parameters) =>
                {
                    var context = c.Resolve<IComponentContext>();
                    var keyObject = parameters.TypedAs<TKeyObject>();
                    return GetInstanceFor(keyObject,context);
                }).As(service);

                yield return rb.CreateRegistration();
            }
           
        }

        private TInstance GetInstanceFor(TKeyObject keyObject, IComponentContext context)
        {
            var component = default(TInstance);
            while (!_internalDictionary.TryGetValue(keyObject, out component))
            {
                component = _instanceFactory(keyObject,context);
                _internalDictionary.TryAdd(keyObject, component);
            }
            return component;
        }
        
    }

    
}
