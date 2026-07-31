using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex013_AgeCalculatorTests
{
    [Theory]
    [InlineData(2000, 1, 1, 2020, 1, 1, 20)]     // exact birthday, no leap involved
    [InlineData(1990, 5, 5, 1990, 5, 5, 0)]      // birth date equals reference date
    [InlineData(2000, 6, 15, 2020, 6, 14, 19)]   // one day before birthday this year
    [InlineData(2000, 6, 15, 2020, 6, 16, 20)]   // one day after birthday this year
    [InlineData(2000, 2, 29, 2021, 2, 28, 20)]   // leap-year birthday, non-leap ref year, before Mar 1
    [InlineData(2000, 2, 29, 2021, 3, 1, 21)]    // leap-year birthday, non-leap ref year, on/after Mar 1
    [InlineData(2000, 2, 29, 2024, 2, 29, 24)]   // leap-year birthday landing on an actual leap day
    public void GetAge_ReturnsExpected(
        int birthYear, int birthMonth, int birthDay,
        int refYear, int refMonth, int refDay,
        int expected)
    {
        var birthDate = new DateTime(birthYear, birthMonth, birthDay);
        var referenceDate = new DateTime(refYear, refMonth, refDay);

        Assert.Equal(expected, AgeCalculator.GetAge(birthDate, referenceDate));
    }
}
