using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Dot.Extension;

namespace Dot.Test.Extension
{
    [TestClass]
    public class IEnumerableExtensionTest
    {
        [TestMethod]
        public void ExtensionMethod_IEnumerable_Split_Test()
        {
            var source = Enumerable.Range(1, 100);

            var splits = source.Split(6);
            var sourceRestore = splits.SelectMany(split => split);
            var totalCount = sourceRestore.Count();
            var min = sourceRestore.Min();
            var max = sourceRestore.Max();
            Assert.AreEqual(source.Min(), min);
            Assert.AreEqual(source.Max(), max);
            Assert.AreEqual(source.Count(), totalCount);

            splits = source.Split(6, IEnumerableSplitStrategy.MaximalAverage);
            sourceRestore = splits.SelectMany(split => split);
            totalCount = sourceRestore.Count();
            min = sourceRestore.Min();
            max = sourceRestore.Max();
            Assert.AreEqual(source.Min(), min);
            Assert.AreEqual(source.Max(), max);
            Assert.AreEqual(source.Count(), totalCount);
        }
    }
}