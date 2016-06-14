using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dot.Threading.Atomic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Test
{
    [TestClass]
    public class AtomicIntegerTest
    {
        [TestMethod]
        public void AtomicInteger_CompareAndSet_ThreadSafe_Test()
        {
            var integer = new AtomicInteger(0);
            var casResult = new ConcurrentDictionary<int, bool>();
            var tasks = Enumerable.Range(1, 50).Select(key => Task.Factory.StartNew(() => casResult.TryAdd(key, integer.CompareAndSet(0, key))));
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(casResult.Values.Count(val => val == true) == 1);
        }

        [TestMethod]
        public void AtomicInteger_Set_Test()
        {
            var integer = new AtomicInteger(0);
            Enumerable.Range(1, 50).Select(key =>
            {
                return Task.Factory.StartNew(() => 
                {
                    var oldVal = integer.Value;
                    var newVal = key;
                    Assert.AreEqual<int>(oldVal, integer.Set(newVal));
                    Assert.AreEqual<int>(newVal, integer.Value);
                });
            });
        }
    }
}