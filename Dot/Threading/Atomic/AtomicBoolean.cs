using System;
using System.Threading;

namespace Dot.Threading.Atomic
{
    public class AtomicBoolean
    {
        private volatile int _value;

        public bool Value
        {
            get { return Convert.ToBoolean(_value); }
        }

        public AtomicBoolean(bool initVal)
        {
            _value = Convert.ToInt32(initVal);
        }

        public bool CompareAndSet(bool currentVal, bool newVal)
        {
#pragma warning disable 420
            int currentInt = Convert.ToInt32(currentVal);
            int newInt = Convert.ToInt32(newVal);
            return Interlocked.CompareExchange(ref _value, newInt, currentInt) == currentInt;
#pragma warning restore 420
        }

        public bool Set(bool newVal)
        {
#pragma warning disable 420
            int newInt = Convert.ToInt32(newVal);
            return Convert.ToBoolean(Interlocked.Exchange(ref _value, newInt));
#pragma warning restore 420
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}