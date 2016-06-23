using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Dependency
{
    public interface IObjectContainer
    {
        void RegisterType(Type serviceType, LifeStyle life = LifeStyle.Singleton);

        void RegisterType(Type serviveType, Type implType, string registerName = "", LifeStyle life = LifeStyle.Singleton);

        void Register<TService, TImpl>(string registerName = "", LifeStyle life = LifeStyle.Singleton)
            where TService : class
            where TImpl : class, TService;

        void RegisterInstance<TService, TImpl>(TImpl instance, string registerName = "")
            where TService : class
            where TImpl : class, TService;

        void RegisterInstance(Type serviceType, object instance, string registerName = "");

        TService Resolve<TService>(string registerName = "") where TService : class;

        object Resolve(Type serviceType, string registerName = "");
    }

    public enum LifeStyle
    {
        /// <summary>
        /// 瞬时
        /// </summary>
        Transient,
        /// <summary>
        /// 单例
        /// </summary>
        Singleton
    }
}