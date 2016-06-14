using System.Threading;

namespace Dot.Threading.Atomic
{
    public class AtomicInteger
    {
        private volatile int _value;

        public int Value
        {
            get { return _value; }
        }

        public AtomicInteger(int initVal)
        {
            _value = initVal;
        }

        public bool CompareAndSet(int currentVal, int newVal)
        {
#pragma warning disable 420
            return Interlocked.CompareExchange(ref _value, newVal, currentVal) == currentVal;
#pragma warning restore 420
        }

        public int Set(int newVal)
        {
#pragma warning disable 420
            return Interlocked.Exchange(ref _value, newVal);
#pragma warning restore 420
        }

        public int Increment()
        {
#pragma warning disable 420
            return Interlocked.Increment(ref _value);
#pragma warning restore 420
        }

        public int GetThenIncrement()
        {
            var value = _value;
            if (this.CompareAndSet(value, value + 1))
                return value;
            else
                return this.GetThenIncrement();
        }

        public int Decrement()
        {
#pragma warning disable 420
            return Interlocked.Decrement(ref _value);
#pragma warning restore 420
        }

        public int GetThenDecrement()
        {
            var value = _value;
            if (this.CompareAndSet(value, value - 1))
                return value;
            else
                return this.GetThenDecrement();
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}