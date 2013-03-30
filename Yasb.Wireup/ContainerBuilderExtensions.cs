using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Builder;
using Autofac;
using Autofac.Core;

namespace Yasb.Wireup
{
    public static class ContainerBuilderExtensions
    {
        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterWithScope<T>(this ContainerBuilder builder, Func<ILifetimeScope,IEnumerable<Parameter>, T> func)
        {
            return builder.Register<T>((c,p)=>
            {
                var lifetimeScope = c.Resolve<ILifetimeScope>();
                return func(lifetimeScope.BeginLifetimeScope(),p);
            });
        }

        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterWithScope<T>(this ContainerBuilder builder, Func<ILifetimeScope,T> func)
        {
            return builder.Register<T>((c) =>
            {
                var lifetimeScope = c.Resolve<ILifetimeScope>();
                return func(lifetimeScope.BeginLifetimeScope());
            });
        }
        public static void RegisterOneInstanceForObjectKey<TKeyObject, T>(this ContainerBuilder builder, Func<TKeyObject, T> func)
            where  TKeyObject : class
        {
            builder.RegisterSource(new RedisSocketRegistrationSource<TKeyObject, T>(func));
            
        }
       
    }
}
