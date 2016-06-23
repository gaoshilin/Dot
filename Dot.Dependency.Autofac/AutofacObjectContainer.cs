using System;
using Autofac;

namespace Dot.Dependency.Autofac
{
    public class AutofacObjectContainer : IObjectContainer
    {
        private readonly IContainer _container;

        public AutofacObjectContainer()
        {
            _container = new ContainerBuilder().Build();
        }

        public AutofacObjectContainer(ContainerBuilder containerBuilder)
        {
            _container = containerBuilder.Build();
        }

        public void RegisterType(Type serviceType, LifeStyle life = LifeStyle.Singleton)
        {
            var builder = new ContainerBuilder();
            var registrationBuilder = builder.RegisterType(serviceType);

            switch (life)
            {
                case LifeStyle.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
                case LifeStyle.Singleton:
                    registrationBuilder.SingleInstance();
                    break;
            }

            builder.Update(_container);
        }

        public void RegisterType(Type serviceType, Type implType, string registerName = "", LifeStyle life = LifeStyle.Singleton)
        {
            var builder = new ContainerBuilder();
            var registrationBuilder = builder.RegisterType(implType).AsSelf();
            if (!string.IsNullOrEmpty(registerName))
                registrationBuilder.Named(registerName, serviceType);
            else
                registrationBuilder.As(serviceType);

            switch (life)
            {
                case LifeStyle.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
                case LifeStyle.Singleton:
                    registrationBuilder.SingleInstance();
                    break;
            }

            builder.Update(_container);
        }

        public void Register<TService, TImpl>(string registerName = "", LifeStyle life = LifeStyle.Singleton)
            where TService : class
            where TImpl : class, TService
        {
            var builder = new ContainerBuilder();
            var registrationBuilder = builder.RegisterType<TImpl>().AsSelf();
            if (!string.IsNullOrEmpty(registerName))
                registrationBuilder.Named<TService>(registerName);
            else
                registrationBuilder.As<TService>();

            switch (life)
            {
                case LifeStyle.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
                case LifeStyle.Singleton:
                    registrationBuilder.SingleInstance();
                    break;
            }

            builder.Update(_container);
        }

        public void RegisterInstance<TService, TImpl>(TImpl instance, string registerName = "")
            where TService : class
            where TImpl : class, TService
        {
            var builder = new ContainerBuilder();
            if (string.IsNullOrEmpty(registerName))
                builder.RegisterInstance(instance).As<TService>().SingleInstance();
            else
                builder.RegisterInstance(instance).Named<TService>(registerName).SingleInstance();

            builder.Update(_container);
        }

        public void RegisterInstance(Type serviceType, object instance, string registerName = "")
        {
            var builder = new ContainerBuilder();
            if (string.IsNullOrEmpty(registerName))
                builder.RegisterInstance(instance).As(serviceType).SingleInstance();
            else
                builder.RegisterInstance(instance).Named(registerName, serviceType).SingleInstance();

            builder.Update(_container);
        }

        public TService Resolve<TService>(string registerName = "")
            where TService : class
        {
            if (!string.IsNullOrEmpty(registerName))
                return _container.ResolveNamed<TService>(registerName);
            else
                return _container.Resolve<TService>();
        }

        public object Resolve(Type serviceType, string registerName = "")
        {
            if (!string.IsNullOrEmpty(registerName))
                return _container.ResolveNamed(registerName, serviceType);
            else
                return _container.Resolve(serviceType);
        }
    }
}