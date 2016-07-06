using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core.Lifetime;
using Dot.Denpendency.Engine;
using Dot.Dependency;
using Dot.Extension;
using Dot.Test.Support;
using Dot.Test.Util;
using Dot.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Test.Dependency
{
    [TestClass]
    public class DotEngineTest
    {
        [TestMethod]
        public void Engine_DotEngine_Test()
        {
            var engine = new DotEngine();

            // Csharp 被注册为 ILanguage 接口的默认实现
            Assert.IsNotNull(engine.Resolve<ILanguage>());
            Assert.IsInstanceOfType(engine.Resolve<ILanguage>(), typeof(Csharp));

            // 瞬时模式注册，通过任意方式解析得到的实例都具有不同的引用
            var csharp1 = engine.Resolve<Csharp>();
            var csharp2 = engine.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag).Resolve<Csharp>();
            var csharp3 = engine.BeginLifetimeScope().Resolve<Csharp>();
            var csharp4 = engine.BeginLifetimeScope("scope1").Resolve<Csharp>();
            var csharps = new List<Csharp> { csharp1, csharp2, csharp3, csharp4 };
            Assert.IsTrue(csharps.AllReferenceNotEqual());

            // 单例模式注册，通过任意方式解析得到的实例都具有相同的引用
            var java1 = engine.Resolve<Java>();
            var java2 = engine.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag).Resolve<Java>();
            var java3 = engine.BeginLifetimeScope().Resolve<Java>();
            var scope1 = engine.BeginLifetimeScope("scope1");
            var java4 = scope1.Resolve<Java>();
            var java5 = scope1.Resolve<Java>();
            var javas = new List<Java> { java1, java2, java3, java4, java5 };
            Assert.IsTrue(javas.AllReferenceEqual());

            // 域共享模式注册，相同域解析得到的实例具有相同的引用，不同域解析得到的实例具有不同的引用
            var phpScope1 = engine.BeginLifetimeScope();
            var phpScope1_php1 = phpScope1.Resolve<Php>();
            var phpScope1_php2 = phpScope1.Resolve<Php>();
            var phpScope2 = engine.BeginLifetimeScope();
            var phpScope2_php1 = phpScope2.Resolve<Php>();
            var phpScope2_php2 = phpScope2.Resolve<Php>();
            Assert.IsTrue(Assert.ReferenceEquals(phpScope2_php1, phpScope2_php2));
            Assert.IsTrue(Assert.ReferenceEquals(phpScope1_php1, phpScope1_php2));
            Assert.IsFalse(Assert.ReferenceEquals(phpScope1_php1, phpScope2_php1));

            // 以指定 name 注册的类，解析时如果不指定 name，将产生异常
            AssertUtil.CatchException(() => engine.Resolve<Ruby>());
            AssertUtil.CatchException(() => engine.ResolveNamed<Ruby>(""));
            Assert.IsNotNull(engine.ResolveNamed<Ruby>("ruby"));
        }
    }
}