using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace Yasb.Wireup
{
    public class LifetimeScopeFactory
    {
        private List<ILifetimeScope> _lifetimeScopesList = new List<ILifetimeScope>();
        private Action<object, ContainerBuilder> _registerer;
        private ILifetimeScope _lifetimeScope;
        public LifetimeScopeFactory(ILifetimeScope lifetimeScope, Action<object,ContainerBuilder> registerer)
        {
            _lifetimeScope = lifetimeScope;
            _registerer = registerer;
        }

        public void EnsureLifetimeScopeFor(object tag) {
            var lifetimeScope = GetLifetimeScopeFor(tag);
            if (lifetimeScope == null)
            {
                lifetimeScope = _lifetimeScope.BeginLifetimeScope(tag, cb => _registerer(tag, cb));
                _lifetimeScopesList.Add(lifetimeScope);
            }
        }

        public ILifetimeScope GetLifetimeScopeFor(object tag)
        {
            return _lifetimeScopesList.FirstOrDefault(lf => lf.Tag.Equals(tag));
        }
    }
}
