namespace FeWoLearning.Exercises.Advanced;

// Exercise 083 — Middleware pipeline (reference solution).
// Each middleware is composed into the one before it: invoking middleware[i]
// passes it a `next` delegate that, when called, invokes middleware[i + 1].
// The composition is built back-to-front so `Execute` only ever needs to
// invoke the first composed delegate.
public sealed class RequestContext
{
    public List<string> Log { get; } = new();

    public bool Handled { get; set; }
}

public sealed class MiddlewarePipeline
{
    private readonly List<Action<RequestContext, Action>> _middlewares = new();

    public MiddlewarePipeline Use(Action<RequestContext, Action> middleware)
    {
        if (middleware is null)
            throw new ArgumentNullException(nameof(middleware));

        _middlewares.Add(middleware);
        return this;
    }

    public void Execute(RequestContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        // Build the chain from the last middleware backward so that each
        // step's `next` closes over the correctly-composed continuation.
        Action next = () => { };
        for (var i = _middlewares.Count - 1; i >= 0; i--)
        {
            var middleware = _middlewares[i];
            var current = next;
            next = () => middleware(context, current);
        }

        next();
    }
}
