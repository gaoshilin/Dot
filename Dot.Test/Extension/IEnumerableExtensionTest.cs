using System;
using System.Collections.Generic;
using System.Linq;
using Dot.Extension;
using Dot.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Test.Extension
{
    [TestClass]
    public class IEnumerableExtensionTest
    {
        [TestMethod]
        public void ExtensionMethod_IEnumerable_Left_Test()
        {
            AssertUtil.CatchException<ArgumentNullException>(() =>
            {
                List<int> nullItems = null;
                nullItems.Left(10).ToList();
            });

            AssertUtil.CatchException<ArgumentException>(() =>
            {
                Enumerable.Range(1, 10).Left(-1).ToList();
            });

            var items = Enumerable.Range(1, 10);

            var left0 = items.Left(0);
            Assert.AreEqual<int>(0, left0.Count());

            var left1 = items.Left(1);
            Assert.AreEqual<int>(1, left1.Count());
            Assert.AreEqual<int>(1, left1.Min());

            var left5 = items.Left(5);
            Assert.AreEqual<int>(5, left5.Count());
            Assert.AreEqual<int>(1, left5.Min());
            Assert.AreEqual<int>(5, left5.Max());

            var left10 = items.Left(10);
            Assert.AreEqual<int>(10, left10.Count());
            Assert.AreEqual<int>(1, left10.Min());
            Assert.AreEqual<int>(10, left10.Max());

            var left100 = items.Left(100);
            Assert.AreEqual<int>(10, left100.Count());
            Assert.AreEqual<int>(1, left100.Min());
            Assert.AreEqual<int>(10, left100.Max());

            Assert.AreEqual<int>(0, Enumerable.Empty<int>().Left(10).Count());
        }

        [TestMethod]
        public void ExtensionMethod_IEnumerable_Right_Test()
        {
            var items = Enumerable.Range(1, 10);

            var right0 = items.Right(0);
            Assert.AreEqual<int>(0, right0.Count());

            var right1 = items.Right(1);
            Assert.AreEqual<int>(1, right1.Count());
            Assert.AreEqual<int>(10, right1.Max());

            var right5 = items.Right(5);
            Assert.AreEqual<int>(5, right5.Count());
            Assert.AreEqual<int>(6, right5.Min());
            Assert.AreEqual<int>(10, right5.Max());

            var right100 = items.Right(100);
            Assert.AreEqual<int>(10, right100.Count());
            Assert.AreEqual<int>(1, right100.Min());
            Assert.AreEqual<int>(10, right100.Max());
        }

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

        [TestMethod]
        public void ExtensionMethod_IEnumerable_MapKeyToValues_Test()
        {
            var source = new List<string>
            {
                "1-1", "1-2", "1-2",
                "2-1", "2-2", "2-3"
            };

            Func<string, string> keySelector = item => item.Split('-')[0];
            Func<string, int> valueSelector = item => Convert.ToInt32(item.Split('-')[1]);

            var map = source.MapKeyToValues(keySelector, valueSelector);
            Assert.AreEqual<int>(2, map.Keys.Count);
            Assert.AreEqual<int>(3, map["1"].Count);

            var mapDistincted = source.MapKeyToValues(keySelector, valueSelector, true);
            Assert.AreEqual<int>(2, mapDistincted.Keys.Count);
            Assert.AreEqual<int>(2, mapDistincted["1"].Count);
        }

        [TestMethod]
        public void ExtensionMethod_IEnumerable_JoinToString_Test()
        {
            var source = Enumerable.Range(1, 3);
            var expectString = string.Join(",", source);
            var actualString = source.JoinToString(",");
            Assert.AreEqual<string>(expectString, actualString);

            var items = source.Select(item => string.Format("{0}ABC", item));
            Func<string, string> selector = item => item.Substring(0, 1);
            var actualString2 = items.JoinToString(",", selector);
            Assert.AreEqual<string>(expectString, actualString2);
        }

        [TestMethod]
        public void ExtensionMethod_IEnumerable_Between_Test()
        {
            var items = Enumerable.Range(1, 10);

            var _0to0 = items.Between(0, 0);
            Assert.AreEqual(1, _0to0.Count());
            Assert.AreEqual(1, _0to0.Min());

            var _0to9 = items.Between(0, 9);
            Assert.AreEqual(10, _0to9.Count());
            Assert.AreEqual(1, _0to9.Min());
            Assert.AreEqual(10, _0to9.Max());

            var _0to90 = items.Between(0, 90);
            Assert.AreEqual(10, _0to90.Count());
            Assert.AreEqual(1, _0to90.Min());
            Assert.AreEqual(10, _0to90.Max());
        }
    }
}