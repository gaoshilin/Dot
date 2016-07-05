using Autofac;
using Dot.Configuration;
using Dot.Dependency;

namespace Dot.Engine
{
    public class DefaultEngine : EngineBase
    {
        public DefaultEngine(DotConfig config = null)
            : base(config)
        {
        }
    }
}