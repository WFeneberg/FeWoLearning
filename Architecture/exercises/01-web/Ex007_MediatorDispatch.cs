namespace FeWoLearning.Architecture.Exercises.Web.Ex007
{
    /// <summary>Marker: a request and the response type it promises.</summary>
    public interface IRequest<TResponse>;

    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IGreeter
    {
        string Greet(string who);
    }

    public sealed class Greeter : IGreeter
    {
        public string Greet(string who) => "hello " + who;
    }

    public sealed record Ping(string Text) : IRequest<string>;

    /// <summary>
    /// Takes a dependency on purpose: a mediator that reaches for
    /// Activator.CreateInstance instead of the container cannot construct this.
    /// </summary>
    public sealed class PingHandler(IGreeter greeter) : IRequestHandler<Ping, string>
    {
        public string Handle(Ping request) => greeter.Greet(request.Text);
    }

    public sealed record Sum(int A, int B) : IRequest<int>;

    public sealed class SumHandler : IRequestHandler<Sum, int>
    {
        public int Handle(Sum request) => request.A + request.B;
    }

    /// <summary>Deliberately has no handler registered anywhere.</summary>
    public sealed record Orphan(string Text) : IRequest<string>;

    // Exercise 007 — MediatorDispatch (web).
    // Goal:   Dispatch a request to its handler using the request's RUNTIME type, and
    //         fail usefully when there is no handler.
    // Drills: mediator, handler resolution by request type, no service locator leak.
    // Passes: Send(new Ping("world"))    - returns "hello world", which is only
    //                    reachable if the handler came from the CONTAINER (PingHandler
    //                    cannot be constructed without IGreeter).
    //         Send(new Sum(2, 3))        - returns 5: a second request with a different
    //                    response type.
    //         Send(new Orphan("x"))      - throws InvalidOperationException whose
    //                    message names "Orphan".
    //         Alpha.Echo vs Beta.Echo    - two request types with the SAME simple name
    //                    in different namespaces reach their own handlers.
    //
    // Note the shape of Send: the request arrives typed as IRequest<TResponse>, so the
    // static type tells you nothing about which handler to use. Everything has to come
    // from request.GetType().
    public sealed class Mediator(IServiceProvider services)
    {
        public TResponse Send<TResponse>(IRequest<TResponse> request) =>
            throw new NotImplementedException(
                "TODO: Ex007 - resolve IRequestHandler<TRequest, TResponse> for request.GetType() from the container and invoke it");
    }
}

namespace FeWoLearning.Architecture.Exercises.Web.Ex007.Alpha
{
    public sealed record Echo(string Text) : IRequest<string>;

    public sealed class EchoHandler : IRequestHandler<Echo, string>
    {
        public string Handle(Echo request) => "alpha:" + request.Text;
    }
}

namespace FeWoLearning.Architecture.Exercises.Web.Ex007.Beta
{
    /// <summary>
    /// Same simple name as Alpha.Echo, on purpose. A mediator that keys its handler
    /// table by Type.Name rather than by Type is an ordinary-looking implementation
    /// that passes everything else and collapses these two onto one handler.
    /// </summary>
    public sealed record Echo(string Text) : IRequest<string>;

    public sealed class EchoHandler : IRequestHandler<Echo, string>
    {
        public string Handle(Echo request) => "beta:" + request.Text;
    }
}
