namespace FeWoLearning.Exercises.Intermediate;

// Exercise 050 — Custom Exception Hierarchy (reference solution).
public abstract class ValidationException : Exception
{
    public string ErrorCode { get; }

    protected ValidationException(string errorCode, string message) : base(message)
        => ErrorCode = errorCode;
}

public sealed class RequiredFieldException : ValidationException
{
    public RequiredFieldException(string fieldName)
        : base("REQUIRED", $"'{fieldName}' is required.")
    {
    }
}

public sealed class OutOfRangeException : ValidationException
{
    public OutOfRangeException(string fieldName, int value, int min, int max)
        : base("OUT_OF_RANGE", $"'{fieldName}' must be between {min} and {max}, but was {value}.")
    {
    }
}

public static class CustomExceptionHierarchy
{
    public static int Validate(string fieldName, string? value, int min, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new RequiredFieldException(fieldName);
        }

        var parsed = int.Parse(value);

        if (parsed < min || parsed > max)
        {
            throw new OutOfRangeException(fieldName, parsed, min, max);
        }

        return parsed;
    }
}
