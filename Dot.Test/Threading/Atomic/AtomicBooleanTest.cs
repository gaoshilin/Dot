using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Dot.Threading.Atomic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Test.Threading.Atomic
{
    [TestClass]
    public class AtomicBooleanTest
    {
        [TestMethod]
        public void AtomicBoolean_CompareAndSet_ThreadSafe_Test()
        {
            var boolean = new AtomicBoolean(false);
            var casResult = new ConcurrentDictionary<int, bool>();
            var tasks = Enumerable.Range(1, 50).Select(key => Task.Factory.StartNew(() => casResult.TryAdd(key, boolean.CompareAndSet(false, true))));
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(casResult.Values.Count(val => val == true) == 1);
        }

        [TestMethod]
        public void AtomicBoolean_Set_ThreadSafe_Test()
        {
            var boolean = new AtomicBoolean(false);
            Enumerable.Range(1, 50).Select(key =>
            {
                return Task.Factory.StartNew(() => 
                {
                    var oldVal = boolean.Value;
                    var newVal = key % 2 == 1;
                    Assert.AreEqual<bool>(oldVal, boolean.Set(newVal));
                    Assert.AreEqual<bool>(newVal, boolean.Value);
                });
            });
        }
    }
}