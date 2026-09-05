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

// Exercise 010 — ResultErrorModel (web).
// Goal:   Move expected failures out of the exception channel and into the return
//         value, and map each of them onto exactly one HTTP status.
// Drills: Result vs exceptions, error-to-status mapping, no control flow by throw.
// Passes: Transfer  - a valid transfer moves the money and returns Success with a
//                     Receipt; an unknown account returns Failure(NotFound); too little
//                     money returns Failure(InsufficientFunds); and a FAILED transfer
//                     leaves BOTH balances exactly as they were.
//         ToStatusCode - NotFound to 404, InsufficientFunds to 409, Validation to 400,
//                     and an ErrorCode value outside the enum throws
//                     ArgumentOutOfRangeException.
//
// The untouched-balances clause is the one that grades the design rather than the
// naming. Debiting first and discovering the shortfall afterwards produces exactly the
// same Failure result, and has already taken the money.
//
// The out-of-range clause grades the mapping the same way: a switch ending in
// `_ => 500` maps every code, including ones nobody has thought about yet, and turns
// the next added ErrorCode into a silent 500 instead of a compile-time decision.
public static class Ex010_ResultErrorModel
{
    /// <summary>
    /// Move <paramref name="amount"/> between two accounts. Validate everything before
    /// changing anything.
    /// </summary>
    public static Result<Receipt> Transfer(AccountStore store, string from, string to, decimal amount) =>
        throw new NotImplementedException(
            "TODO: Ex010 - validate both accounts and the balance BEFORE moving any money, and return a Result rather than throwing");

    /// <summary>Map an error code to its HTTP status. No catch-all.</summary>
    public static int ToStatusCode(ErrorCode code) =>
        throw new NotImplementedException(
            "TODO: Ex010 - map each ErrorCode explicitly and throw ArgumentOutOfRangeException for anything else");
}
