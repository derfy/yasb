using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Builder;
using Autofac;
using Autofac.Core;
using Yasb.Common.Messaging;
using Yasb.Common.Serialization.MessageDeserializers;

namespace Yasb.Wireup
{
    public static class ContainerBuilderExtensions
    {
       
        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterWithScope<T>(this ContainerBuilder builder, Func<ILifetimeScope, IEnumerable<Parameter>, T> func, object scope = null)
        {
            return builder.Register((c,p)=>
            {
                var lifetimeScope = scope == null ? c.Resolve<ILifetimeScope>() : c.Resolve<ILifetimeScope>().BeginLifetimeScope(scope);
                return func(lifetimeScope,p);
            });
        }

        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterWithScope<T>(this ContainerBuilder builder, Func<ILifetimeScope,T> func,object scope =null)
        {
            return builder.Register((c) =>
            {
                var lifetimeScope = scope==null ? c.Resolve<ILifetimeScope>() : c.Resolve<ILifetimeScope>().BeginLifetimeScope(scope);
                return func(lifetimeScope);
            });
        }
        public static void RegisterOneInstanceForObjectKey<TKeyObject, T>(this ContainerBuilder builder, Func<TKeyObject, IComponentContext, T> func)
            where  TKeyObject : class
        {
            builder.RegisterSource(new OneInstancePerKeyObjectRegistrationSource<TKeyObject, T>(func));
            
        }
    }
}
