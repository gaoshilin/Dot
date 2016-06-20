using System;
using Dot.Util;

namespace Dot.Extension
{
    public static class DecimalExtension
    {
        public static decimal KeepDigits(this decimal value, int digits = 2, bool rounding = true)
        {
            Ensure.Greater(digits, 0, "digits");

            if (rounding)
            {
                return Math.Round(value, digits, MidpointRounding.AwayFromZero);
            }
            else
            {
                var parts = value.ToString().Split('.');
                return parts.Length == 2
                     ? Convert.ToDecimal("{0}.{1}".FormatWith(parts[0], parts[1].Left(digits)))
                     : value;
            }
        }
    }
}