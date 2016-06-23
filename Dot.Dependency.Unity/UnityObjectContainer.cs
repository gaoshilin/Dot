using System;
using Microsoft.Practices.Unity;

namespace Dot.Dependency.Unity
{
    public class UnityObjectContainer : IObjectContainer
    {
        public IUnityContainer Container { get; private set; }

        public UnityObjectContainer()
        {
            Container = new UnityContainer();
        }

        public void RegisterType(Type implType, LifeStyle life = LifeStyle.Singleton)
        {
            Container.RegisterType(implType, life.ToLifeTimeManager());
        }

        public void RegisterType(Type serviceType, Type implType, string registerName = "", LifeStyle life = LifeStyle.Singleton)
        {
            if (!string.IsNullOrEmpty(registerName))
                Container.RegisterType(serviceType, implType, registerName, life.ToLifeTimeManager());
            else
                Container.RegisterType(serviceType, implType, life.ToLifeTimeManager());
        }

        public void Register<TService, TImpl>(string registerName = "", LifeStyle life = LifeStyle.Singleton)
            where TService : class
            where TImpl : class, TService
        {
            if (!string.IsNullOrEmpty(registerName))
                Container.RegisterType(typeof(TService), typeof(TImpl), registerName, life.ToLifeTimeManager());
            else
                Container.RegisterType(typeof(TService), typeof(TImpl), life.ToLifeTimeManager());
        }

        public void RegisterInstance(Type serviceType, object instance, string registerName = "")
        {
            if (!string.IsNullOrEmpty(registerName))
                Container.RegisterInstance(serviceType, registerName, instance, new ContainerControlledLifetimeManager());
            else
                Container.RegisterInstance(serviceType, instance, new ContainerControlledLifetimeManager());
        }

        public void RegisterInstance<TService, TImpl>(TImpl instance, string registerName = "")
            where TService : class
            where TImpl : class, TService
        {
            if (!string.IsNullOrEmpty(registerName))
                Container.RegisterInstance(typeof(TService), registerName, instance, new ContainerControlledLifetimeManager());
            else
                Container.RegisterInstance(typeof(TService), instance, new ContainerControlledLifetimeManager());
        }

        public TService Resolve<TService>(string registerName = "") where TService : class
        {
            if (!string.IsNullOrEmpty(registerName))
                return Container.Resolve<TService>(registerName);
            else
                return Container.Resolve<TService>();
        }

        public object Resolve(Type serviceType, string registerName = "")
        {
            return Container.Resolve(serviceType, registerName);
        }
    }

    static class LifeStyleExtension
    {
        internal static LifetimeManager ToLifeTimeManager(this LifeStyle life)
        {
            if (life == LifeStyle.Transient)
                return new TransientLifetimeManager();

            if (life == LifeStyle.Singleton)
                return new ContainerControlledLifetimeManager();

            throw new ArgumentException("life");
        }
    }
}