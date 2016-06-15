using System;

namespace Dot.Util
{
    public static class DecimalUtil
    {
        /// <summary>
        /// 保留指定位小数，四舍五入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits">保留小数位</param>
        /// <param name="rounding">是否四舍五入</param>
        /// <returns></returns>
        public static decimal KeepDigits(this decimal value, int digits = 2, bool rounding = true)
        {
            if (digits < 0)
                throw new ArgumentException("digits must be greater or equal than 0.", "digits");

            if (rounding)
            {
                return Math.Round(value, digits, MidpointRounding.AwayFromZero);
            }
            else
            {
                var chunks = value.ToString().Split('.');
                if (chunks.Length == 1)
                {
                    return value;
                }
                else
                {
                    var integerPart = chunks[0];
                    var digitsPart = chunks[1];
                    digits = Math.Min(digits, digitsPart.Length);
                    return Convert.ToDecimal(string.Format("{0}.{1}", integerPart, digitsPart.Substring(0, digits)));
                }
            }
        }
    }
}