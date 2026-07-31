namespace FeWoLearning.Exercises.Advanced;

// Exercise 084 — Result/Option pattern (advanced).
// Goal:   Implement a Result<T> type with Success/Failure states and Map/Bind
//         combinators so error handling composes without exceptions or nulls.
// Drills: discriminated-union-style types, functional combinators, short-circuiting.
public readonly struct Result<T>
{
    private Result(bool isSuccess, T value, string error)
    {
        throw new NotImplementedException();
    }

    public bool IsSuccess => throw new NotImplementedException();

    public bool IsFailure => throw new NotImplementedException();

    public T Value => throw new NotImplementedException();

    public string Error => throw new NotImplementedException();

    public static Result<T> Success(T value) => throw new NotImplementedException();

    public static Result<T> Failure(string error) => throw new NotImplementedException();

    // Transforms the success value, leaving a failure untouched (and never
    // invoking the mapper on a failed result).
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper) => throw new NotImplementedException();

    // Chains another Result-producing operation, short-circuiting on failure.
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder) => throw new NotImplementedException();

    // Unwraps to a plain value, substituting a fallback for failures.
    public T GetValueOrDefault(T fallback) => throw new NotImplementedException();
}

public static class ResultOptionPattern
{
    // Parses an int, returning a Failure Result instead of throwing on bad input.
    public static Result<int> ParseInt(string input) => throw new NotImplementedException();

    // Divides two ints as a Result, failing on division by zero instead of throwing.
    public static Result<int> Divide(int numerator, int denominator) => throw new NotImplementedException();

    // Parses "a/b" and computes a/b in one chained pipeline using Bind/Map,
    // short-circuiting on the first failure encountered.
    public static Result<int> ParseAndDivide(string expression) => throw new NotImplementedException();
}
