namespace FeWoLearning.Architecture.Exercises.Web.Ex010;

public enum ErrorCode
{
    NotFound,
    InsufficientFunds,
    Validation,
}

public readonly record struct Error(ErrorCode Code, string Message);

/// <summary>
/// Given, not a TODO: this exercise is about the error MODEL, not about writing a
/// result type. Reading Value on a failure throws, so a caller cannot skip the check
/// and silently get default(T).
/// </summary>
public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly Error _error;

    private Result(bool isSuccess, T? value, Error error) =>
        (IsSuccess, _value, _error) = (isSuccess, value, error);

    public bool IsSuccess { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot read Value of a failed result.");

    public Error Error => IsSuccess
        ? throw new InvalidOperationException("Cannot read Error of a successful result.")
        : _error;

    public static Result<T> Success(T value) => new(true, value, default);

    public static Result<T> Failure(ErrorCode code, string message) =>
        new(false, default, new Error(code, message));
}

public sealed record Receipt(string From, string To, decimal Amount);

public sealed class AccountStore
{
    private readonly Dictionary<string, decimal> _balances = [];

    public void Seed(string account, decimal balance) => _balances[account] = balance;

    public bool Exists(string account) => _balances.ContainsKey(account);

    public decimal BalanceOf(string account) => _balances[account];

    public void Adjust(string account, decimal delta) => _balances[account] += delta;
}

// Exercise 010 — ResultErrorModel (reference solution).
public static class Ex010_ResultErrorModel
{
    public static Result<Receipt> Transfer(AccountStore store, string from, string to, decimal amount)
    {
        // Every check happens before the first Adjust. That ordering is the design:
        // once money has moved, "return a failure" is no longer an honest description
        // of what happened.
        if (amount <= 0)
            return Result<Receipt>.Failure(ErrorCode.Validation, "Amount must be positive.");

        if (!store.Exists(from))
            return Result<Receipt>.Failure(ErrorCode.NotFound, $"Unknown account '{from}'.");

        if (!store.Exists(to))
            return Result<Receipt>.Failure(ErrorCode.NotFound, $"Unknown account '{to}'.");

        if (store.BalanceOf(from) < amount)
            return Result<Receipt>.Failure(ErrorCode.InsufficientFunds,
                $"Account '{from}' holds less than {amount}.");

        store.Adjust(from, -amount);
        store.Adjust(to, amount);

        return Result<Receipt>.Success(new Receipt(from, to, amount));
    }

    public static int ToStatusCode(ErrorCode code) => code switch
    {
        ErrorCode.NotFound => 404,
        ErrorCode.InsufficientFunds => 409,
        ErrorCode.Validation => 400,
        // Not `_ => 500`. A catch-all maps codes nobody has thought about yet, which
        // turns the next ErrorCode someone adds into a silent 500 in production instead
        // of a decision made here.
        _ => throw new ArgumentOutOfRangeException(nameof(code), code, "Unmapped error code."),
    };
}
