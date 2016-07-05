using System;

namespace Dot.Dependency
{
    [Flags]
    public enum RegisterMode
    {
        Self = 1,
        Interface = 2,
        DefaultInterface = 4
    }
}