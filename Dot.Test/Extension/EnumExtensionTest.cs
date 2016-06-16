using System;
using System.Linq;
using Dot.Extension;
using Dot.Test.Support;
using Dot.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Test.Extension
{
    [TestClass]
    public class EnumExtensionTest
    {
        [TestMethod]
        public void ExtensionMethod_Enum_Test()
        {
            AssertUtil.CatchException<ArgumentException>(() => typeof(EnumExtensionTest).GetEnumDatas());

            var datas = typeof(ProductType).GetEnumDatas();
            Assert.AreEqual(3, datas.Count);
            Assert.AreEqual(ProductType.Normal.Name(), datas.ElementAt(0).Name);
            Assert.AreEqual(ProductType.Combo.Value(), datas.ElementAt(1).Value);
            Assert.AreEqual(ProductType.Countdown.Description(), datas.ElementAt(2).Description);
            Assert.AreEqual(ProductType.Normal, ProductType.Normal.Name().ToEnumByName<ProductType>());
            Assert.AreEqual(ProductType.Combo, ProductType.Combo.Value().ToEnumByValue<ProductType>());
            Assert.AreEqual(ProductType.Countdown, ProductType.Countdown.Description().ToEnumByDescription<ProductType>());

            var range = ProductType.Normal | ProductType.Combo;
            Assert.IsTrue(ProductType.Normal.InEnumRange(range));
            Assert.IsTrue(range.ContainsEnum(ProductType.Combo));
            Assert.IsFalse(ProductType.Countdown.InEnumRange(range));
            Assert.IsFalse(range.ContainsEnum(ProductType.Countdown));
        }
    }
}