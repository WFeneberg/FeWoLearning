namespace FeWoLearning.Exercises.Advanced;

// Exercise 084 — Result/Option pattern (reference solution).
public readonly struct Result<T>
{
    private readonly T _value;
    private readonly string _error;

    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        _value = value;
        _error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access Value of a failed Result.");

    public string Error => IsFailure
        ? _error
        : throw new InvalidOperationException("Cannot access Error of a successful Result.");

    public static Result<T> Success(T value) => new(true, value, null!);

    public static Result<T> Failure(string error) => new(false, default!, error);

    public Result<TResult> Map<TResult>(Func<T, TResult> mapper) =>
        IsSuccess ? Result<TResult>.Success(mapper(_value)) : Result<TResult>.Failure(_error);

    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder) =>
        IsSuccess ? binder(_value) : Result<TResult>.Failure(_error);

    public T GetValueOrDefault(T fallback) => IsSuccess ? _value : fallback;
}

public static class ResultOptionPattern
{
    public static Result<int> ParseInt(string input) =>
        int.TryParse(input, out var n)
            ? Result<int>.Success(n)
            : Result<int>.Failure($"'{input}' is not a valid integer.");

    public static Result<int> Divide(int numerator, int denominator) =>
        denominator == 0
            ? Result<int>.Failure("Division by zero.")
            : Result<int>.Success(numerator / denominator);

    public static Result<int> ParseAndDivide(string expression)
    {
        var parts = expression.Split('/');
        if (parts.Length != 2)
            return Result<int>.Failure($"'{expression}' is not in the form 'a/b'.");

        return ParseInt(parts[0])
            .Bind(numerator => ParseInt(parts[1])
                .Bind(denominator => Divide(numerator, denominator)));
    }
}
