using Autofac;
using Autofac.Core.Lifetime;
using Dot.Dependency;
using Dot.Test.Support;
using Dot.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Test.Dependency
{
    [TestClass]
    public class DotEngineTest
    {
        [TestMethod]
        public void Autofac_Test4()
        {
            var datas = Registration.Scan(AssemblyUtil.GetAssemblies(false)).ToArray();
            var builder = new ContainerBuilder();
            builder.Register(datas);
            var container = builder.Build();

            Assert.IsNotNull(container.Resolve<ILanguage>());
            Assert.IsInstanceOfType(container.Resolve<ILanguage>(), typeof(Csharp));

            var csharp1 = container.ResolveNamed<Csharp>("csharp");
            var csharp2 = container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag).ResolveNamed<Csharp>("csharp");
            var csharp3 = container.BeginLifetimeScope().ResolveNamed<Csharp>("csharp");
            Assert.IsTrue(object.ReferenceEquals(csharp1, csharp2));
            Assert.IsTrue(object.ReferenceEquals(csharp1, csharp3));

            //var scope1 = container.BeginLifetimeScope();
            //var php1 = scope1.ResolveNamed<Php>("php");
            //var php2 = scope1.ResolveNamed<Php>("php");
            //Assert.ReferenceEquals(php1, php2);

            //var scope2 = container.BeginLifetimeScope();
            //var php3 = scope2.ResolveNamed<Php>("php");
            //var php4 = scope2.ResolveNamed<Php>("php");
            //Assert.ReferenceEquals(php3, php4);
            //Assert.AreNotEqual(php1, php3);

            //using (var scope3 = container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag))
            //{
            //    var java1 = scope3.ResolveNamed<Java>("java");
            //    var java2 = scope3.ResolveNamed<Java>("java");
            //    var java3 = container.ResolveNamed<Java>("java");
            //    var java4 = container.ResolveNamed<Java>("java");

            //    Assert.IsFalse(Assert.ReferenceEquals(java1, java2));
            //    Assert.IsFalse(Assert.ReferenceEquals(java1, java3));
            //    Assert.IsFalse(Assert.ReferenceEquals(java3, java4));
            //}

            //var ruby = container.Resolve<Ruby>();
            //Assert.IsNotNull(ruby);
        }
    }    
}