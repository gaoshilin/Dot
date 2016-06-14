using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Threading.Atomic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

namespace Dot.Test
{
    [TestClass]
    public class AtomicEnumTest
    {
        [TestMethod]
        public void AtomicEnum_CompareAndSet_ThreadSafe_Test()
        {
            AtomicEnum<Warehouse> atomic = new AtomicEnum<Warehouse>(Warehouse.Sjgo);
            var casResult = new ConcurrentDictionary<int, bool>();
            var tasks = Enumerable.Range(1, 50).Select(key => 
            {
                return Task.Factory.StartNew(() => 
                {
                    casResult.TryAdd(key, atomic.CompareAndSet(Warehouse.Sjgo, Warehouse.Kgil));
                });
            });
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(casResult.Values.Count(val => val == true) == 1);
        }
    }

    public enum Warehouse
    {
        Sjgo = 1,
        Kgil = 2,
        Gangteng = 3
    }
}