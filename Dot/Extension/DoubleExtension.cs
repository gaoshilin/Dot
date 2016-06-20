using System;
using Dot.Util;

namespace Dot.Extension
{
    public static class DoubleExtension
    {
        public static double KeepDigits(this double value, int digits = 2, bool rounding = true)
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
                     ? Convert.ToDouble("{0}.{1}".FormatWith(parts[0], parts[1].Left(digits)))
                     : value;
            }
        }
    }
}