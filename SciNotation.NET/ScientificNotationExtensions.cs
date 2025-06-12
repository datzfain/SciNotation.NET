using System;
using System.Globalization;

namespace SciNotation.NET
{
    /// <summary>
    /// Extension methods to convert various numeric types and strings
    /// into a human-readable scientific notation: "mantissa × 10^exponent".
    /// </summary>
    public static class ScientificNotationExtensions
    {
        /// <summary>
        /// Formats an object (string, numeric, or integer) into scientific notation.
        /// </summary>
        /// <param name="value">
        /// The value to format; must be a string, double, float, decimal, or integer type.
        /// </param>
        /// <param name="decimals">
        /// Number of digits after the decimal point in the mantissa.
        /// </param>
        /// <param name="provider">
        /// Culture info for decimal separators and parsing; defaults to InvariantCulture.
        /// </param>
        /// <returns>
        /// A string of the form "mantissa × 10^exponent", or "0", "NaN", "∞", "-∞" for special cases.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="value"/> is null or of an unsupported type,
        /// or if a string input cannot be parsed to a number.
        /// </exception>
        public static string ToScientificNotation(this object value, int decimals = 6, IFormatProvider? provider = null)
        {
            if (value is null)
                throw new ArgumentException("Value cannot be null", nameof(value));

            provider ??= CultureInfo.InvariantCulture;
            switch (value)
            {
                case string s:
                    try
                    {
                        return s.ToScientificNotation(decimals, provider);
                    }
                    catch (FormatException)
                    {
                        throw new ArgumentException($"Input string '{s}' is not a valid number", nameof(value));
                    }

                case double d:
                    return d.ToScientificNotation(decimals, provider);

                case float f:
                    return f.ToScientificNotation(decimals, provider);

                case decimal dec:
                    return dec.ToScientificNotation(decimals, provider);

                // All integer types are converted to double
                case sbyte _:
                case byte _:
                case short _:
                case ushort _:
                case int _:
                case uint _:
                case long _:
                case ulong _:
                    double dv = Convert.ToDouble(value, provider);
                    return dv.ToScientificNotation(decimals, provider);

                default:
                    throw new ArgumentException($"Type {value.GetType().Name} is not supported", nameof(value));
            }
        }

        /// <summary>
        /// Formats a <see cref="double"/> into scientific notation.
        /// </summary>
        /// <param name="value">The double value to format.</param>
        /// <param name="decimals">
        /// Number of digits after the decimal point in the mantissa.
        /// </param>
        /// <param name="provider">
        /// Culture info for formatting; defaults to InvariantCulture.
        /// </param>
        /// <returns>
        /// A string of the form "mantissa × 10^exponent", or "0", "NaN", "∞", "-∞".
        /// </returns>
        public static string ToScientificNotation(this double value, int decimals = 6, IFormatProvider? provider = null)
        {
            if (double.IsNaN(value)) return "NaN";
            if (double.IsPositiveInfinity(value)) return "∞";
            if (double.IsNegativeInfinity(value)) return "-∞";
            if (value == 0) return "0";

            provider ??= CultureInfo.InvariantCulture;
            var nfi = NumberFormatInfo.GetInstance(provider);

            // Determine exponent as floor(log10(|value|))
            int exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)));

            double pow10;
            try
            {
                pow10 = Math.Pow(10, exponent);
            }
            catch
            {
                pow10 = double.PositiveInfinity;
            }

            double mantissaRaw = value / pow10;

            // Handle subnormal values where mantissaRaw becomes Infinity or NaN
            if (double.IsInfinity(mantissaRaw) || double.IsNaN(mantissaRaw))
            {
                // Use "E" format as a fallback: e.g. "-4.940656E-324"
                string sci = value.ToString("E" + decimals, provider);
                string[] parts = sci.Split('E');
                string m = parts[0]
                    .TrimEnd('0')
                    .TrimEnd(nfi.NumberDecimalSeparator[0]);
                int exp = int.Parse(parts[1], NumberStyles.Integer, provider);
                return $"{m} × 10^{exp}";
            }

            // Round mantissa to the requested number of decimals
            double rounded = Math.Round(mantissaRaw, decimals);

            // Format mantissa: drop decimal point if integer after rounding
            string mantissaStr = rounded == Math.Truncate(rounded)
                ? rounded.ToString("F0", provider)
                : rounded.ToString("F" + decimals, provider);

            return $"{mantissaStr} × 10^{exponent}";
        }

        /// <summary>
        /// Formats a <see cref="float"/> into scientific notation.
        /// </summary>
        /// <param name="value">The float value to format.</param>
        /// <param name="decimals">
        /// Number of digits after the decimal point in the mantissa.
        /// </param>
        /// <param name="provider">
        /// Culture info for formatting; defaults to InvariantCulture.
        /// </param>
        /// <returns>
        /// A string of the form "mantissa × 10^exponent", or "0", "NaN", "∞", "-∞".
        /// </returns>
        public static string ToScientificNotation(this float value, int decimals = 6, IFormatProvider? provider = null)
        {
            if (float.IsNaN(value)) return "NaN";
            if (float.IsPositiveInfinity(value)) return "∞";
            if (float.IsNegativeInfinity(value)) return "-∞";
            if (value == 0f) return "0";

            // Delegate to double implementation for consistency
            return ((double)value).ToScientificNotation(decimals, provider);
        }

        /// <summary>
        /// Formats a <see cref="decimal"/> into scientific notation.
        /// </summary>
        /// <param name="value">The decimal value to format.</param>
        /// <param name="decimals">
        /// Number of digits after the decimal point in the mantissa.
        /// </param>
        /// <param name="provider">
        /// Culture info for formatting; defaults to InvariantCulture.
        /// </param>
        /// <returns>
        /// A string of the form "mantissa × 10^exponent", or "0" if value is zero.
        /// </returns>
        public static string ToScientificNotation(this decimal value, int decimals = 6, IFormatProvider? provider = null)
        {
            if (value == 0m) return "0";
            provider ??= CultureInfo.InvariantCulture;
            // Use double for exponent calculation but preserve decimal precision in rounding
            return ((double)value).ToScientificNotation(decimals, provider);
        }

        /// <summary>
        /// Parses a numeric string and formats it into scientific notation.
        /// </summary>
        /// <param name="value">The numeric string to parse.</param>
        /// <param name="decimals">
        /// Number of digits after the decimal point in the mantissa.
        /// </param>
        /// <param name="provider">
        /// Culture info for parsing and formatting; defaults to InvariantCulture.
        /// </param>
        /// <returns>
        /// A string of the form "mantissa × 10^exponent".
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the input string is null, empty, or whitespace.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown if the input string cannot be parsed to a number.
        /// </exception>
        public static string ToScientificNotation(this string value, int decimals = 6, IFormatProvider? provider = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Input string cannot be null or whitespace", nameof(value));

            provider ??= CultureInfo.InvariantCulture;
            if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, provider, out double d))
                return d.ToScientificNotation(decimals, provider);
            if (decimal.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, provider, out decimal dec))
                return dec.ToScientificNotation(decimals, provider);

            throw new FormatException($"Input string '{value}' is not a valid number");
        }
    }
}
