using System;

namespace Dot.Sample.Support
{
    public class Person
    {
        public int Id;
        public int Weight;
        public override string ToString()
        {
            return string.Format("Id = {0}, Weight = {1}", Id, Weight);
        }
    }
}