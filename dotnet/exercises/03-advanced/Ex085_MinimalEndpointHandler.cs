namespace FeWoLearning.Exercises.Advanced;

// Exercise 085 — Minimal-API-style endpoint handler (advanced).
// Goal:   Implement a pure handler function mimicking a minimal API route:
//         it validates an incoming request DTO and returns either a success
//         result carrying a response DTO, or a validation-error result
//         carrying field-level error messages (no HTTP framework involved).
// Drills: DTO validation, discriminated-union-style result modelling,
//         pure functions decoupled from a web framework.

// Incoming request payload (as would be bound from a JSON body).
public sealed record CreateUserRequest(string? Name, string? Email, int Age);

// Outgoing payload on success.
public sealed record CreateUserResponse(string Name, string Email, int Age);

// Uniform result of the handler: exactly one of Response / Errors is populated.
public sealed class EndpointResult
{
    public bool IsSuccess => throw new NotImplementedException();
    public CreateUserResponse? Response => throw new NotImplementedException();
    public IReadOnlyList<string> Errors => throw new NotImplementedException();

    public static EndpointResult Success(CreateUserResponse response) => throw new NotImplementedException();

    public static EndpointResult Failure(IReadOnlyList<string> errors) => throw new NotImplementedException();
}

public static class MinimalEndpointHandler
{
    // Mimics a minimal API route handler: `app.MapPost("/users", Handle)`.
    // Validates the request and, if valid, maps it to a response DTO.
    // Validation rules:
    //   - Name must be non-null/non-whitespace.
    //   - Email must be non-null/non-whitespace and contain '@' and '.'.
    //   - Age must be between 0 and 149 inclusive.
    // All violated rules are reported together (not just the first one).
    public static EndpointResult Handle(CreateUserRequest request) => throw new NotImplementedException();
}
