using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Configuration;
using Dot.Pattern;

namespace Dot.Dependency.Engine
{
    public static class EngineContext
    {
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                    Initialize(false);
                return Singleton<IEngine>.Instance;
            }
        }

        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                var config = ConfigurationManager.GetSection("dotConfig") as DotConfig;
                var args = new object[] { config };
                var engine = Activator.CreateInstance(Type.GetType(config.EngineType), args) as IEngine;
                Singleton<IEngine>.Instance = engine;
            }

            return Singleton<IEngine>.Instance;
        }
    }
}