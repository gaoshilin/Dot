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
                if (parts.Length == 1)
                {
                    return value;
                }
                else
                {
                    var integerPart = parts[0];
                    var digitsPart = parts[1];
                    return Convert.ToDouble("{0}.{1}".FormatWith(integerPart, digitsPart.Left(digits)));
                }
            }
        }
    }
}