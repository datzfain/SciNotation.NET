using System;
using System.Globalization;
using System.Text;
using System.Collections.Generic;

namespace SciNotation.NET
{
    /// <summary>
    /// Extension methods to convert various numeric types and strings
    /// into a human-readable scientific notation: "mantissa × 10ⁿ".
    /// </summary>
    public static class ScientificNotationExtensions
    {
        // Mapping of characters to their superscript Unicode equivalents
        private static readonly Dictionary<char, char> _superscriptMap = new Dictionary<char, char>
        {
            ['0'] = '⁰',
            ['1'] = '¹',
            ['2'] = '²',
            ['3'] = '³',
            ['4'] = '⁴',
            ['5'] = '⁵',
            ['6'] = '⁶',
            ['7'] = '⁷',
            ['8'] = '⁸',
            ['9'] = '⁹',
            ['-'] = '⁻'
        };

        /// <summary>
        /// Converts an integer to its superscript representation (e.g., -12 → "⁻¹²").
        /// </summary>
        private static string ToSuperscript(int value)
        {
            string s = value.ToString(CultureInfo.InvariantCulture);
            var sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (_superscriptMap.TryGetValue(c, out char sup))
                    sb.Append(sup);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Formats an object into scientific notation.
        /// Supports string, double, float, decimal, and integer types.
        /// </summary>
        public static string ToScientificNotation(this object value, int decimals = 6, IFormatProvider? provider = null)
        {
            if (value is null)
                throw new ArgumentException("Value cannot be null", nameof(value));

            provider ??= CultureInfo.InvariantCulture;
            switch (value)
            {
                case string s:
                    try { return s.ToScientificNotation(decimals, provider); }
                    catch (FormatException) { throw new ArgumentException($"Input string '{s}' is not a valid number", nameof(value)); }
                case double d:
                    return d.ToScientificNotation(decimals, provider);
                case float f:
                    return f.ToScientificNotation(decimals, provider);
                case decimal dec:
                    return dec.ToScientificNotation(decimals, provider);
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
        /// Formats a double into scientific notation: "mantissa × 10ⁿ".
        /// </summary>
        public static string ToScientificNotation(this double value, int decimals = 6, IFormatProvider? provider = null)
        {
            if (double.IsNaN(value)) return "NaN";
            if (double.IsPositiveInfinity(value)) return "∞";
            if (double.IsNegativeInfinity(value)) return "-∞";
            if (value == 0) return "0";

            provider ??= CultureInfo.InvariantCulture;
            var nfi = NumberFormatInfo.GetInstance(provider);

            // Calculate exponent
            int exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)));
            double pow10;
            try { pow10 = Math.Pow(10, exponent); }
            catch { pow10 = double.PositiveInfinity; }

            double mantissaRaw = value / pow10;
            // Fallback for subnormal values
            if (double.IsInfinity(mantissaRaw) || double.IsNaN(mantissaRaw))
            {
                string sci = value.ToString("E" + decimals, provider);
                string[] parts = sci.Split('E');
                string m = parts[0].TrimEnd('0').TrimEnd(nfi.NumberDecimalSeparator[0]);
                int exp = int.Parse(parts[1], NumberStyles.Integer, provider);
                return $"{m} × 10{ToSuperscript(exp)}";
            }

            double rounded = Math.Round(mantissaRaw, decimals);
            string mantissaStr = rounded == Math.Truncate(rounded)
                ? rounded.ToString("F0", provider)
                : rounded.ToString("F" + decimals, provider);

            return $"{mantissaStr} × 10{ToSuperscript(exponent)}";
        }

        /// <summary>
        /// Formats a float into scientific notation by delegating to double.
        /// </summary>
        public static string ToScientificNotation(this float value, int decimals = 6, IFormatProvider? provider = null)
        {
            if (float.IsNaN(value)) return "NaN";
            if (float.IsPositiveInfinity(value)) return "∞";
            if (float.IsNegativeInfinity(value)) return "-∞";
            if (value == 0f) return "0";
            return ((double)value).ToScientificNotation(decimals, provider);
        }

        /// <summary>
        /// Formats a decimal into scientific notation by delegating to double.
        /// </summary>
        public static string ToScientificNotation(this decimal value, int decimals = 6, IFormatProvider? provider = null)
        {
            if (value == 0m) return "0";
            provider ??= CultureInfo.InvariantCulture;
            return ((double)value).ToScientificNotation(decimals, provider);
        }

        /// <summary>
        /// Parses a numeric string and formats it into scientific notation.
        /// </summary>
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