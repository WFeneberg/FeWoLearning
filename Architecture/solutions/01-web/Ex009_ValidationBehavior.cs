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

// Exercise 009 — ValidationBehavior (reference solution).
public static class Ex009_ValidationBehavior
{
    public static string Execute(
        CreateUser request,
        IReadOnlyList<IValidator<CreateUser>> validators,
        CreateUserHandler handler)
    {
        // SelectMany over every validator, not a loop that returns on the first
        // failure. A caller fixing one field at a time and re-submitting is the cost of
        // fail-fast, and it is paid by a person, once per problem.
        var errors = validators.SelectMany(v => v.Validate(request)).ToList();

        if (errors.Count > 0)
            throw new RequestValidationException(errors);

        return handler.Handle(request);
    }
}
