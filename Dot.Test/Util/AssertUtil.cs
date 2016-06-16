using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Test.Util
{
    public static class AssertUtil
    {
        public static void CatchException<T>(Action action) where T : Exception
        {
            bool catchException = false;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                catchException = ex.GetType() == typeof(T);
            }
            Assert.IsTrue(catchException);
        }

        public static void CatchException(Action action)
        {
            bool catchException = false;
            try
            {
                action();
            }
            catch
            {
                catchException = true;
            }
            Assert.IsTrue(catchException);
        }
    }
}