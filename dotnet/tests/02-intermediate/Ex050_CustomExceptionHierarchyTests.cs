using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex050_CustomExceptionHierarchyTests
{
    [Fact]
    public void Validate_ReturnsParsedValue_WhenWithinRange()
    {
        var result = CustomExceptionHierarchy.Validate("age", "42", 0, 120);

        Assert.Equal(42, result);
    }

    [Fact]
    public void Validate_ThrowsRequiredFieldException_WhenValueIsNull()
    {
        var ex = Assert.Throws<RequiredFieldException>(
            () => CustomExceptionHierarchy.Validate("age", null, 0, 120));

        Assert.Equal("REQUIRED", ex.ErrorCode);
        Assert.Equal("'age' is required.", ex.Message);
        Assert.IsAssignableFrom<ValidationException>(ex);
    }

    [Fact]
    public void Validate_ThrowsRequiredFieldException_WhenValueIsWhitespace()
    {
        var ex = Assert.Throws<RequiredFieldException>(
            () => CustomExceptionHierarchy.Validate("name", "   ", 0, 120));

        Assert.Equal("REQUIRED", ex.ErrorCode);
    }

    [Fact]
    public void Validate_ThrowsOutOfRangeException_WhenValueTooLow()
    {
        var ex = Assert.Throws<OutOfRangeException>(
            () => CustomExceptionHierarchy.Validate("age", "-1", 0, 120));

        Assert.Equal("OUT_OF_RANGE", ex.ErrorCode);
        Assert.Equal("'age' must be between 0 and 120, but was -1.", ex.Message);
    }

    [Fact]
    public void Validate_ThrowsOutOfRangeException_WhenValueTooHigh()
    {
        var ex = Assert.Throws<OutOfRangeException>(
            () => CustomExceptionHierarchy.Validate("age", "200", 0, 120));

        Assert.Equal("OUT_OF_RANGE", ex.ErrorCode);
        Assert.Equal("'age' must be between 0 and 120, but was 200.", ex.Message);
    }

    [Fact]
    public void Validate_DoesNotThrowRequiredFieldException_ForOutOfRangeValue()
    {
        Assert.Throws<OutOfRangeException>(
            () => CustomExceptionHierarchy.Validate("age", "200", 0, 120));
    }
}
