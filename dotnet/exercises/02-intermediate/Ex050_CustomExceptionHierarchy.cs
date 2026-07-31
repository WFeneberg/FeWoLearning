namespace FeWoLearning.Exercises.Intermediate;

// Exercise 050 — Custom Exception Hierarchy (intermediate).
// Goal:   Model input validation failures using a custom exception hierarchy
//         instead of generic exceptions. Define a base ValidationException
//         carrying an ErrorCode, and two subtypes:
//           - RequiredFieldException  (code "REQUIRED")   — value is null/empty/whitespace.
//           - OutOfRangeException     (code "OUT_OF_RANGE") — value is outside [min, max].
//         Validate(fieldName, value, min, max) must throw the correct subtype
//         with a clear, specific message, or return normally when the value is valid.
// Drills: custom exception hierarchies, exception constructors, error codes,
//         throwing/propagating specific exception types.
public abstract class ValidationException : Exception
{
    public string ErrorCode { get; }

    protected ValidationException(string errorCode, string message) : base(message)
        => throw new NotImplementedException();
}

public sealed class RequiredFieldException : ValidationException
{
    public RequiredFieldException(string fieldName)
        : base("REQUIRED", $"'{fieldName}' is required.")
        => throw new NotImplementedException();
}

public sealed class OutOfRangeException : ValidationException
{
    public OutOfRangeException(string fieldName, int value, int min, int max)
        : base("OUT_OF_RANGE", $"'{fieldName}' must be between {min} and {max}, but was {value}.")
        => throw new NotImplementedException();
}

public static class CustomExceptionHierarchy
{
    // Throws RequiredFieldException if value is null/empty/whitespace,
    // OutOfRangeException if the parsed integer is outside [min, max],
    // otherwise returns the parsed integer.
    public static int Validate(string fieldName, string? value, int min, int max)
        => throw new NotImplementedException();
}
