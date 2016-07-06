using Autofac;
using Dot.Configuration;
using Dot.Dependency;

namespace Dot.Denpendency.Engine
{
    public class DotEngine : EngineBase
    {
        public DotEngine() : base() { }

        public DotEngine(DotConfig config = null) : base(config) { }

        protected override void CustomRegister(ContainerBuilder builder, DotConfig config)
        {
        }
    }
}