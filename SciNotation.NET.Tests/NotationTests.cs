using Xunit;
using System;
using System.Globalization;

namespace SciNotation.NET.Tests
{
    /// <summary>
    /// Unit tests for the ToScientificNotation extension methods across various numeric types and strings.
    /// </summary>
    public class NotationTests
    {
        #region String Tests
        /// <summary>
        /// Validates conversion from string representations (including scientific input) into human-readable scientific notation.
        /// </summary>
        [Theory]
        [InlineData("0", "0")]                            // Zero input should return "0"
        [InlineData("5", "5 × 10⁰")]                      // Integer input without decimal point
        [InlineData("0.000000005", "5 × 10⁻⁹")]           // Small decimal input
        [InlineData("-12345", "-1.234500 × 10⁴")]         // Negative integer input
        [InlineData("123.456", "1.234560 × 10²")]         // Positive decimal input
        [InlineData("1e10", "1 × 10¹⁰")]                  // Scientific notation input
        public void String_ToScientificNotation_Works(string input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Ensures FormatException is thrown for non-numeric strings and ArgumentException for empty input.
        /// </summary>
        [Fact]
        public void String_ToScientificNotation_ThrowsForInvalidInput()
        {
            Assert.Throws<FormatException>(() => "not a number".ToScientificNotation());
            Assert.Throws<ArgumentException>(() => string.Empty.ToScientificNotation());
        }

        /// <summary>
        /// Verifies that the method respects culture-specific formatting (e.g., comma decimal separator).
        /// </summary>
        [Fact]
        public void String_ToScientificNotation_RespectsCulture()
        {
            var culture = new CultureInfo("de-DE"); // German culture: comma as decimal separator
            string result = "1234,56".ToScientificNotation(provider: culture);
            Assert.Equal("1,234560 × 10³", result);
        }
        #endregion

        #region Double Tests
        /// <summary>
        /// Validates scientific notation conversion for double values, including extreme values.
        /// </summary>
        [Theory]
        [InlineData(0.0, "0")]                              // Zero double
        [InlineData(5.0, "5 × 10⁰")]                        // Simple positive double
        [InlineData(0.000000005, "5 × 10⁻⁹")]               // Small double
        [InlineData(-12345.0, "-1.234500 × 10⁴")]           // Negative double
        [InlineData(123.456, "1.234560 × 10²")]             // Positive decimal double
        [InlineData(double.Epsilon, "4.940656 × 10⁻³²⁴")]    // Smallest positive double
        [InlineData(double.MaxValue, "1.797693 × 10³⁰⁸")]    // Largest double value
        public void Double_ToScientificNotation_Works(double input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Ensures special double values (NaN, Infinity) are handled correctly.
        /// </summary>
        [Fact]
        public void Double_ToScientificNotation_HandlesSpecialValues()
        {
            Assert.Equal("NaN", double.NaN.ToScientificNotation());
            Assert.Equal("∞", double.PositiveInfinity.ToScientificNotation());
            Assert.Equal("-∞", double.NegativeInfinity.ToScientificNotation());
        }
        #endregion

        #region Decimal Tests
        /// <summary>
        /// Validates scientific notation conversion for decimal values with high precision.
        /// </summary>
        [Theory]
        [InlineData(0.0, "0")]                              // Zero decimal
        [InlineData(5.0, "5 × 10⁰")]                        // Simple positive decimal
        [InlineData(0.000000005, "5 × 10⁻⁹")]               // Small decimal
        [InlineData(-12345.0, "-1.234500 × 10⁴")]           // Negative decimal
        [InlineData(123.456, "1.234560 × 10²")]             // Positive decimal
        public void Decimal_ToScientificNotation_Works(decimal input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Tests conversion for the maximum decimal value with limited decimal places.
        /// </summary>
        [Fact]
        public void Decimal_ToScientificNotation_HandlesLargeValues()
        {
            decimal maxValue = decimal.MaxValue;
            string result = maxValue.ToScientificNotation(decimals: 2);
            Assert.Equal("7.92 × 10²⁸", result);
        }
        #endregion

        #region Integer Types Tests
        /// <summary>
        /// Validates scientific notation for signed integer types (int, long, short, byte).
        /// </summary>
        [Theory]
        [InlineData(0, "0")]                                // Zero int
        [InlineData(5, "5 × 10⁰")]                          // Small positive int
        [InlineData(-42, "-4.200000 × 10¹")]               // Negative int
        [InlineData(int.MaxValue, "2.147484 × 10⁹")]       // Largest int
        public void Int_ToScientificNotation_Works(int input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1000L, "1 × 10³")]                      // Simple long
        [InlineData(-999999999L, "-10 × 10⁸")]              // Negative long with rounding
        public void Long_ToScientificNotation_Works(long input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((short)100, "1 × 10²")]                  // Short positive
        [InlineData((short)-32000, "-3.2 × 10⁴")]           // Short negative with limited precision
        public void Short_ToScientificNotation_Works(short input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 1);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((byte)100, "1 × 10²")]                   // Byte input
        [InlineData((byte)255, "2.55 × 10²")]                // Maximum byte value
        public void Byte_ToScientificNotation_Works(byte input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 2);
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Unsigned Integer Types Tests
        /// <summary>
        /// Validates scientific notation for unsigned integer types (uint, ulong, ushort).
        /// </summary>
        [Theory]
        [InlineData(123u, "1.23 × 10²")]                     // Small uint with limited decimals
        [InlineData(uint.MaxValue, "4.29 × 10⁹")]            // Maximum uint with rounding
        public void UInt_ToScientificNotation_Works(uint input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 2);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1000ul, "1 × 10³")]                      // Simple ulong
        [InlineData(ulong.MaxValue, "1.844674 × 10¹⁹")]      // Maximum ulong
        public void ULong_ToScientificNotation_Works(ulong input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((ushort)100, "1 × 10²")]                  // Simple ushort
        [InlineData(ushort.MaxValue, "6.5535 × 10⁴")]         // Maximum ushort with precision
        public void UShort_ToScientificNotation_Works(ushort input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 4);
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Float Tests
        /// <summary>
        /// Validates conversion for float values, including smallest positive float.
        /// </summary>
        [Theory]
        [InlineData(3.14f, "3.14 × 10⁰")]                  // Simple float
        [InlineData(1000000f, "1 × 10⁶")]                   // Large float
        [InlineData(float.Epsilon, "1.40 × 10⁻⁴⁵")]           // Smallest positive float
        public void Float_ToScientificNotation_Works(float input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 2);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Ensures special float values (NaN, Infinity) are handled correctly.
        /// </summary>
        [Fact]
        public void Float_ToScientificNotation_HandlesSpecialValues()
        {
            Assert.Equal("NaN", float.NaN.ToScientificNotation());
            Assert.Equal("∞", float.PositiveInfinity.ToScientificNotation());
            Assert.Equal("-∞", float.NegativeInfinity.ToScientificNotation());
        }
        #endregion

        #region Generic Object Tests
        /// <summary>
        /// Tests ToScientificNotation on boxed values of different supported numeric types.
        /// </summary>
        [Fact]
        public void Object_ToScientificNotation_WorksWithVariousTypes()
        {
            Assert.Equal("5 × 10⁰", ((object)5).ToScientificNotation());
            Assert.Equal("1.230000 × 10²", ((object)123.0).ToScientificNotation());
            Assert.Equal("1 × 10³", ((object)1000L).ToScientificNotation());
            Assert.Equal("3.140000 × 10⁰", ((object)3.14f).ToScientificNotation());
            Assert.Equal("1.234560 × 10²", ((object)123.456m).ToScientificNotation());
        }

        /// <summary>
        /// Ensures unsupported types throw ArgumentException.
        /// </summary>
        [Fact]
        public void Object_ToScientificNotation_ThrowsForUnsupportedTypes()
        {
            Assert.Throws<ArgumentException>(() => ((object)DateTime.Now).ToScientificNotation());
            Assert.Throws<ArgumentException>(() => ((object)"not a number").ToScientificNotation());
        }
        #endregion

        #region Edge Cases
        /// <summary>
        /// Verifies handling of different decimal place settings, including zero decimals.
        /// </summary>
        [Fact]
        public void ToScientificNotation_HandlesDifferentDecimalPlaces()
        {
            Assert.Equal("1.2 × 10²", 123.456.ToScientificNotation(1));
            Assert.Equal("1.2346 × 10²", 123.456.ToScientificNotation(4));
            Assert.Equal("1 × 10²", 123.456.ToScientificNotation(0));
        }

        /// <summary>
        /// Tests correct rounding behavior and boundary conditions (e.g., rounding 9.999 up to 10).
        /// </summary>
        [Fact]
        public void ToScientificNotation_HandlesRounding()
        {
            Assert.Equal("1.235 × 10²", 123.456.ToScientificNotation(3));
            Assert.Equal("10 × 10⁰", 9.999.ToScientificNotation(1));
        }
        #endregion
    }
}
