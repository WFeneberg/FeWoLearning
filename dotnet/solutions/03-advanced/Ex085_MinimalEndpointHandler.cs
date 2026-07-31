namespace FeWoLearning.Exercises.Advanced;

// Exercise 085 — Minimal-API-style endpoint handler (reference solution).

public sealed record CreateUserRequest(string? Name, string? Email, int Age);

public sealed record CreateUserResponse(string Name, string Email, int Age);

public sealed class EndpointResult
{
    private readonly CreateUserResponse? _response;
    private readonly IReadOnlyList<string> _errors;

    private EndpointResult(CreateUserResponse? response, IReadOnlyList<string> errors)
    {
        _response = response;
        _errors = errors;
    }

    public bool IsSuccess => _response is not null;
    public CreateUserResponse? Response => _response;
    public IReadOnlyList<string> Errors => _errors;

    public static EndpointResult Success(CreateUserResponse response) => new(response, Array.Empty<string>());

    public static EndpointResult Failure(IReadOnlyList<string> errors) => new(null, errors);
}

public static class MinimalEndpointHandler
{
    public static EndpointResult Handle(CreateUserRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Email) ||
            !request.Email.Contains('@') ||
            !request.Email.Contains('.'))
            errors.Add("Email must be a valid address.");

        if (request.Age < 0 || request.Age > 149)
            errors.Add("Age must be between 0 and 149.");

        if (errors.Count > 0)
            return EndpointResult.Failure(errors);

        return EndpointResult.Success(new CreateUserResponse(request.Name!, request.Email!, request.Age));
    }
}
