using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dot.Collections.Concurrent
{
    public class ConcurrentHashSet<T> : IEnumerable<T>
    {
        private ConcurrentDictionary<T, byte> _set;
        private static byte VALUE = (byte)1;

        public ConcurrentHashSet()
        {
            _set = new ConcurrentDictionary<T, byte>();
        }

        public bool TryAdd(T entry)
        {
            return _set.TryAdd(entry, VALUE);
        }

        public bool TryRemove(T entry)
        {
            byte value;
            return _set.TryRemove(entry, out value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _set.Keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _set.Keys.GetEnumerator();
        }
    }
}