namespace Dot.Util
{
    public static class IntegerUtil
    {
        /// <summary>
        /// 求最大公约数
        /// </summary>
        public static int GetGreatestCommonDivisor(int x, int y)
        {
            return y == 0 ? x : GetGreatestCommonDivisor(y, x % y);
        }
    }
}