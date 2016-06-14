using System;
using System.Threading;

namespace Dot.Threading.Atomic
{
    public class AtomicReference<T>
        where T : class
    {
        private volatile T value;

        public T Value
        {
            get { return value; }
        }

        public AtomicReference()
            : this(default(T))
        {
        }

        public AtomicReference(T initVal)
        {
            this.value = initVal;
        }

        public bool CompareAndSet(T currentVal, T newVal)
        {
#pragma warning disable 420
            return Interlocked.CompareExchange<T>(ref value, newVal, currentVal) == currentVal;
#pragma warning restore 420
        }

        public T Set(T newVal)
        {
#pragma warning disable 420
            return Interlocked.Exchange<T>(ref value, newVal);
#pragma warning restore 420
        }

        public T Mutate(Func<T, T> mutator)
        {
            T baseVal = value;
            while (true)
            {
                T newVal = mutator(baseVal);
#pragma warning disable 420
                T currentVal = Interlocked.CompareExchange<T>(ref value, newVal, baseVal);
#pragma warning restore 420

                if (currentVal == baseVal)
                    return baseVal;
                else
                    baseVal = currentVal;
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}