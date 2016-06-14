using System;
using System.Threading;
using Dot.Extension;

namespace Dot.Threading.Atomic
{
    public class AtomicEnum<T>
        where T : struct, IConvertible
    {
        private volatile int value;

        public T Value
        {
            get { return value.ToEnumByValue<T>(); }
        }

        public AtomicEnum(T initVal)
        {
            if (typeof(T).IsEnum == false)
                throw new ArgumentException("", "initVal");

            this.value = initVal.ChangeType<int>();
        }

        public bool CompareAndSet(T currentVal, T newVal)
        {
#pragma warning disable 420
            int curCode = currentVal.ChangeType<int>();
            int newCode = newVal.ChangeType<int>();
            return Interlocked.CompareExchange(ref value, newCode, curCode) == curCode;
#pragma warning restore 420
        }

        public T Set(T newVal)
        {
#pragma warning disable 420
            int newCode = newVal.ChangeType<int>();
            return Interlocked.Exchange(ref value, newCode).ToEnumByValue<T>();
#pragma warning restore 420
        }
    }
}