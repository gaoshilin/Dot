using System.Collections.Generic;

namespace Dot.Database
{
    public class DatabaseSetting
    {
        public string Name { get; set; }
        public string Write { get; set; }
        public List<string> Reads { get; set; }
    }
}