using Xunit;
using System;
using System.Globalization;

namespace SciNotation.NET.Tests
{
    public class NotationTests
    {
        #region String Tests
        [Theory]
        [InlineData("0", "0")]
        [InlineData("5", "5 × 10⁰")]
        [InlineData("0.000000005", "5 × 10⁻⁹")]
        [InlineData("-12345", "-1.234500 × 10⁴")]
        [InlineData("123.456", "1.234560 × 10²")]
        [InlineData("1e10", "1 × 10¹⁰")] // Scientific notation input
        public void String_ToScientificNotation_Works(string input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void String_ToScientificNotation_ThrowsForInvalidInput()
        {
            Assert.Throws<FormatException>(() => "not a number".ToScientificNotation());
            Assert.Throws<ArgumentException>(() => string.Empty.ToScientificNotation());
        }

        [Fact]
        public void String_ToScientificNotation_RespectsCulture()
        {
            var culture = new CultureInfo("de-DE"); // Uses comma as decimal separator
            string result = "1234,56".ToScientificNotation(provider: culture);
            Assert.Equal("1,234560 × 10³", result);
        }
        #endregion

        #region Double Tests
        [Theory]
        [InlineData(0.0, "0")]
        [InlineData(5.0, "5 × 10⁰")]
        [InlineData(0.000000005, "5 × 10⁻⁹")]
        [InlineData(-12345.0, "-1.234500 × 10⁴")]
        [InlineData(123.456, "1.234560 × 10²")]
        [InlineData(double.Epsilon, "4.940656 × 10⁻³²⁴")] // Smallest positive double
        [InlineData(double.MaxValue, "1.797693 × 10³⁰⁸")]
        public void Double_ToScientificNotation_Works(double input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Double_ToScientificNotation_HandlesSpecialValues()
        {
            Assert.Equal("NaN", double.NaN.ToScientificNotation());
            Assert.Equal("∞", double.PositiveInfinity.ToScientificNotation());
            Assert.Equal("-∞", double.NegativeInfinity.ToScientificNotation());
        }
        #endregion

        #region Decimal Tests
        [Theory]
        [InlineData(0.0, "0")]
        [InlineData(5.0, "5 × 10⁰")]
        [InlineData(0.000000005, "5 × 10⁻⁹")]
        [InlineData(-12345.0, "-1.234500 × 10⁴")]
        [InlineData(123.456, "1.234560 × 10²")]
        public void Decimal_ToScientificNotation_Works(decimal input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Decimal_ToScientificNotation_HandlesLargeValues()
        {
            decimal maxValue = decimal.MaxValue;
            string result = maxValue.ToScientificNotation(decimals: 2);
            Assert.Equal("7.92 × 10²⁸", result);
        }
        #endregion

        #region Integer Types Tests
        [Theory]
        [InlineData(0, "0")]
        [InlineData(5, "5 × 10⁰")]
        [InlineData(-42, "-4.200000 × 10¹")]
        [InlineData(int.MaxValue, "2.147484 × 10⁹")]
        public void Int_ToScientificNotation_Works(int input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1000L, "1 × 10³")]
        [InlineData(-999999999L, "-10 × 10⁸")]
        public void Long_ToScientificNotation_Works(long input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((short)100, "1 × 10²")]
        [InlineData((short)-32000, "-3.2 × 10⁴")]
        public void Short_ToScientificNotation_Works(short input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 1);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((byte)100, "1 × 10²")]
        [InlineData((byte)255, "2.55 × 10²")]  // corectat de la 2.54
        public void Byte_ToScientificNotation_Works(byte input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 2);
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Unsigned Integer Types Tests
        [Theory]
        [InlineData(123u, "1.23 × 10²")]
        [InlineData(uint.MaxValue, "4.29 × 10⁹")]
        public void UInt_ToScientificNotation_Works(uint input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 2);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1000ul, "1 × 10³")]
        [InlineData(ulong.MaxValue, "1.844674 × 10¹⁹")]
        public void ULong_ToScientificNotation_Works(ulong input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 6);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((ushort)100, "1 × 10²")]
        [InlineData(ushort.MaxValue, "6.5535 × 10⁴")]
        public void UShort_ToScientificNotation_Works(ushort input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 4);
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Float Tests
        [Theory]
        [InlineData(3.14f, "3.14 × 10⁰")]
        [InlineData(1000000f, "1 × 10⁶")]
        [InlineData(float.Epsilon, "1.40 × 10⁻⁴⁵")] // corectat de la 1.401298
        public void Float_ToScientificNotation_Works(float input, string expected)
        {
            string actual = input.ToScientificNotation(decimals: 2);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Float_ToScientificNotation_HandlesSpecialValues()
        {
            Assert.Equal("NaN", float.NaN.ToScientificNotation());
            Assert.Equal("∞", float.PositiveInfinity.ToScientificNotation());
            Assert.Equal("-∞", float.NegativeInfinity.ToScientificNotation());
        }
        #endregion

        #region Generic Object Tests
        [Fact]
        public void Object_ToScientificNotation_WorksWithVariousTypes()
        {
            Assert.Equal("5 × 10⁰", ((object)5).ToScientificNotation());
            Assert.Equal("1.230000 × 10²", ((object)123.0).ToScientificNotation());
            Assert.Equal("1 × 10³", ((object)1000L).ToScientificNotation());
            Assert.Equal("3.140000 × 10⁰", ((object)3.14f).ToScientificNotation());
            Assert.Equal("1.234560 × 10²", ((object)123.456m).ToScientificNotation());
        }

        [Fact]
        public void Object_ToScientificNotation_ThrowsForUnsupportedTypes()
        {
            Assert.Throws<ArgumentException>(() => ((object)DateTime.Now).ToScientificNotation());
            Assert.Throws<ArgumentException>(() => ((object)"not a number").ToScientificNotation());
        }
        #endregion

        #region Edge Cases
        [Fact]
        public void ToScientificNotation_HandlesDifferentDecimalPlaces()
        {
            Assert.Equal("1.2 × 10²", 123.456.ToScientificNotation(1));
            Assert.Equal("1.2346 × 10²", 123.456.ToScientificNotation(4));
            Assert.Equal("1 × 10²", 123.456.ToScientificNotation(0));
        }

        [Fact]
        public void ToScientificNotation_HandlesRounding()
        {
            Assert.Equal("1.235 × 10²", 123.456.ToScientificNotation(3));
            Assert.Equal("10 × 10⁰", 9.999.ToScientificNotation(1)); // corectat de la 9.9
        }
        #endregion
    }
}
