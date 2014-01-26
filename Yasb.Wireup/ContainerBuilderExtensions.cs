using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Builder;
using Autofac;
using Autofac.Core;
using Yasb.Common.Messaging;

namespace Yasb.Wireup
{
    public static class ContainerBuilderExtensions
    {
       
        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterWithScope<T>(this ContainerBuilder builder, Func<IComponentContext, IEnumerable<Parameter>, T> func)
        {
           

            return builder.Register((c,parameters)=>
            {
                var lifetimeScope = c.Resolve<ILifetimeScope>();
                return func(lifetimeScope, parameters);
            });
        }

        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterWithScope<T>(this ContainerBuilder builder, Func<ILifetimeScope,T> func)
        {
            return builder.Register((c) =>
            {
                var lifetimeScope = c.Resolve<ILifetimeScope>();
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
