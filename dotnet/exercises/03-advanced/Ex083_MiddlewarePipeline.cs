namespace FeWoLearning.Exercises.Advanced;

// Exercise 083 — Middleware pipeline (advanced).
// Goal:   Build a chainable middleware pipeline that runs registered delegates
//         over a shared request context, in registration order, where any
//         middleware can short-circuit the chain by declining to call `next`.
// Drills: delegate composition, closures, the "onion" middleware pattern
//         (as used by ASP.NET Core, Express, etc.).
public sealed class RequestContext
{
    public List<string> Log { get; } = new();

    public bool Handled { get; set; }
}

public sealed class MiddlewarePipeline
{
    // Registers a middleware. Each middleware receives the context and a
    // `next` delegate; calling `next()` continues the chain, not calling it
    // short-circuits (later middlewares never run). Returns `this` to allow
    // fluent chaining of `.Use(...).Use(...)`.
    public MiddlewarePipeline Use(Action<RequestContext, Action> middleware) => throw new NotImplementedException();

    // Runs the full pipeline against the given context, starting with the
    // first registered middleware.
    public void Execute(RequestContext context) => throw new NotImplementedException();
}
