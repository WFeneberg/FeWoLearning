namespace FeWoLearning.Architecture.Exercises.Web.Ex009;

public sealed record CreateUser(string Email, int Age);

public interface IValidator<T>
{
    IEnumerable<string> Validate(T instance);
}

public sealed class EmailValidator : IValidator<CreateUser>
{
    public const string Message = "Email must contain '@'.";

    public IEnumerable<string> Validate(CreateUser instance)
    {
        if (!instance.Email.Contains('@'))
            yield return Message;
    }
}

public sealed class AgeValidator : IValidator<CreateUser>
{
    public const string Message = "Age must be 18 or over.";

    public IEnumerable<string> Validate(CreateUser instance)
    {
        if (instance.Age < 18)
            yield return Message;
    }
}

/// <summary>Counts its own invocations - that count is how "before" is proven.</summary>
public sealed class CreateUserHandler
{
    public int Invocations { get; private set; }

    public string Handle(CreateUser request)
    {
        Invocations++;
        return "user:" + request.Email;
    }
}

public sealed class RequestValidationException(IReadOnlyList<string> errors)
    : Exception("Validation failed: " + string.Join(" ", errors))
{
    public IReadOnlyList<string> Errors { get; } = errors;
}

// Exercise 009 — ValidationBehavior (web).
// Goal:   Run validation as a stage that sits BEFORE the handler, and report every
//         problem the request has rather than the first one found.
// Drills: validation as a pipeline stage, error aggregation, fail-fast vs collect-all.
// Passes: valid request   - the handler runs exactly once and its result is returned.
//         invalid request - a RequestValidationException is thrown and the handler's
//                           Invocations stays at ZERO.
//         two problems    - the exception's Errors carries BOTH messages.
//         one problem     - Errors carries only that one; a validator that found
//                           nothing contributes nothing.
//
// Invocations == 0 is the fact that matters. Asserting only "an invalid request
// produces an error" is satisfied by validating inside the handler, which is a
// different design with different consequences: the handler has already opened its
// transaction, taken its lock, and charged the caller's rate limit.
public static class Ex009_ValidationBehavior
{
    /// <summary>
    /// Run every validator, gather every message, and if there are any, throw
    /// <see cref="RequestValidationException"/> without touching the handler. Otherwise
    /// return the handler's result.
    /// </summary>
    public static string Execute(
        CreateUser request,
        IReadOnlyList<IValidator<CreateUser>> validators,
        CreateUserHandler handler) =>
        throw new NotImplementedException(
            "TODO: Ex009 - run every validator, collect every message, and only call the handler when there are none");
}
