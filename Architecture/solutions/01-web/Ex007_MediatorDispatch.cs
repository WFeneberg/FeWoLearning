using System.Reflection;

namespace FeWoLearning.Architecture.Exercises.Web.Ex007
{
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

    public sealed class PingHandler(IGreeter greeter) : IRequestHandler<Ping, string>
    {
        public string Handle(Ping request) => greeter.Greet(request.Text);
    }

    public sealed record Sum(int A, int B) : IRequest<int>;

    public sealed class SumHandler : IRequestHandler<Sum, int>
    {
        public int Handle(Sum request) => request.A + request.B;
    }

    public sealed record Orphan(string Text) : IRequest<string>;

    // Exercise 007 — MediatorDispatch (reference solution).
    public sealed class Mediator(IServiceProvider services)
    {
        public TResponse Send<TResponse>(IRequest<TResponse> request)
        {
            ArgumentNullException.ThrowIfNull(request);

            // request.GetType(), not typeof(...): the static type here is the interface,
            // and closing the handler over the runtime type is the entire mechanism.
            var handlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(request.GetType(), typeof(TResponse));

            var handler = services.GetService(handlerType)
                ?? throw new InvalidOperationException(
                    $"No handler registered for request type {request.GetType().Name}.");

            var handle = handlerType.GetMethod(nameof(IRequestHandler<Ping, string>.Handle))!;

            try
            {
                return (TResponse)handle.Invoke(handler, [request])!;
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            {
                // Reflection wraps whatever the handler threw. Unwrap it, or every
                // caller has to know the mediator used reflection - which is exactly
                // the implementation detail it exists to hide.
                System.Runtime.ExceptionServices.ExceptionDispatchInfo
                    .Capture(ex.InnerException).Throw();
                throw; // unreachable
            }
        }
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
    public sealed record Echo(string Text) : IRequest<string>;

    public sealed class EchoHandler : IRequestHandler<Echo, string>
    {
        public string Handle(Echo request) => "beta:" + request.Text;
    }
}
