using System;
using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex036_RomanNumeralsTests
{
    [Theory]
    [InlineData(1, "I")]
    [InlineData(4, "IV")]
    [InlineData(9, "IX")]
    [InlineData(58, "LVIII")]
    [InlineData(1994, "MCMXCIV")]
    [InlineData(3999, "MMMCMXCIX")]
    public void ToRoman_ReturnsExpected(int value, string expected)
        => Assert.Equal(expected, RomanNumerals.ToRoman(value));

    [Theory]
    [InlineData(0)]
    [InlineData(4000)]
    public void ToRoman_RejectsOutOfRange(int value)
        => Assert.Throws<ArgumentOutOfRangeException>(() => RomanNumerals.ToRoman(value));
}
