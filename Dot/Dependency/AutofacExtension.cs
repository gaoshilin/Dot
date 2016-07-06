using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Dot.Extension;

namespace Dot.Dependency
{
    public static class AutofacExtension
    {
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> UseLifeCycle<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> rb, LifeCycle lifeTime)
        {
            switch (lifeTime)
            {
                case LifeCycle.Scope:
                    rb.InstancePerLifetimeScope();
                    break;
                case LifeCycle.Singelton:
                    rb.SingleInstance();
                    break;
                case LifeCycle.Transient:
                default:
                    rb.InstancePerDependency();
                    break;
            }

            return rb;
        }

        public static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(this ContainerBuilder builder, Type type, RegisterMode mode, string name)
        {
            var rb = builder.RegisterType(type);

            if ((RegisterMode.Self & mode) == RegisterMode.Self)
            {
                if (!string.IsNullOrEmpty(name))
                    rb.Named(name, type);
                else
                    rb.AsSelf();
            }

            if ((RegisterMode.DefaultInterface & mode) == RegisterMode.DefaultInterface)
                rb.AsImplementedInterfaces();
            else if ((RegisterMode.Interface & mode) == RegisterMode.Interface)
                rb.AsImplementedInterfaces().PreserveExistingDefaults();

            return rb;
        }

        public static IRegistrationBuilder<Func<T>, SimpleActivatorData, SingleRegistrationStyle> Register<T>(this ContainerBuilder builder, Func<T> creator, RegisterMode mode, string name) where T : class
        {
            var rb = builder.Register(c => creator);

            if ((RegisterMode.Self & mode) == RegisterMode.Self)
            {
                if (!string.IsNullOrEmpty(name))
                    rb.Named(name, typeof(T));
                else
                    rb.AsSelf();
            }

            if ((RegisterMode.DefaultInterface & mode) == RegisterMode.DefaultInterface)
                rb.AsImplementedInterfaces();
            else if ((RegisterMode.Interface & mode) == RegisterMode.Interface)
                rb.AsImplementedInterfaces().PreserveExistingDefaults();

            return rb;
        }

        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T>(this ContainerBuilder builder, T instance, RegisterMode mode, string name) where T : class
        {
            var rb = builder.RegisterInstance<T>(instance);

            if ((RegisterMode.Self & mode) == RegisterMode.Self)
            {
                if (!string.IsNullOrEmpty(name))
                    rb.Named(name, instance.GetType());
                else
                    rb.AsSelf();
            }

            if ((RegisterMode.DefaultInterface & mode) == RegisterMode.DefaultInterface)
                rb.AsImplementedInterfaces();
            else if ((RegisterMode.Interface & mode) == RegisterMode.Interface)
                rb.AsImplementedInterfaces().PreserveExistingDefaults();

            return rb;
        }

        public static void Register(this ContainerBuilder builder, params Registration[] datas)
        {
            datas.ForEach(data => builder.Register(data));
        }

        public static void Register(this ContainerBuilder builder, Registration data)
        {
            builder.RegisterType(data.ServiceType, data.RegisterMode, data.Name).UseLifeCycle(data.LifeCycle);
        }
    }
}