using FeWoLearning.Architecture.Exercises.Web.Ex007;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex007_MediatorDispatchTests
{
    private static ServiceProvider Container()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, Greeter>();
        services.AddTransient<IRequestHandler<Ping, string>, PingHandler>();
        services.AddTransient<IRequestHandler<Sum, int>, SumHandler>();
        services.AddTransient<IRequestHandler<Exercises.Web.Ex007.Alpha.Echo, string>,
                              Exercises.Web.Ex007.Alpha.EchoHandler>();
        services.AddTransient<IRequestHandler<Exercises.Web.Ex007.Beta.Echo, string>,
                              Exercises.Web.Ex007.Beta.EchoHandler>();
        // Orphan is deliberately absent.
        return services.BuildServiceProvider();
    }

    [Fact]
    public void Mechanism_The_Handler_Comes_From_The_Container()
    {
        // PingHandler cannot be constructed without IGreeter, so this result is
        // unreachable for a mediator that new's its handlers up with
        // Activator.CreateInstance. The greeting text is the proof.
        using var provider = Container();

        var result = new Mediator(provider).Send(new Ping("world"));

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void A_Second_Request_With_A_Different_Response_Type_Dispatches_Too()
    {
        using var provider = Container();

        Assert.Equal(5, new Mediator(provider).Send(new Sum(2, 3)));
    }

    [Fact]
    public void Dispatch_Uses_The_Runtime_Type_Not_The_Static_One()
    {
        // The variable's static type carries no handler information at all. Making that
        // explicit here because it is the constraint the whole implementation follows
        // from.
        using var provider = Container();
        IRequest<string> request = new Ping("world");

        Assert.Equal("hello world", new Mediator(provider).Send(request));
    }

    [Fact]
    public void Adversarial_An_Unhandled_Request_Fails_With_A_Message_Naming_It()
    {
        // Not NullReferenceException, and not a silent default(TResponse). A mediator
        // is a lookup, and a lookup that misses has to say what it was looking for.
        using var provider = Container();

        var failure = Assert.Throws<InvalidOperationException>(
            () => new Mediator(provider).Send(new Orphan("x")));

        Assert.Contains(nameof(Orphan), failure.Message);
    }

    [Fact]
    public void Adversarial_Two_Request_Types_Sharing_A_Simple_Name_Reach_Their_Own_Handlers()
    {
        // The plausible-wrong catch: a handler table keyed by Type.Name is an ordinary
        // implementation that passes every fact above, and collapses these two onto
        // whichever one was registered last.
        using var provider = Container();
        var mediator = new Mediator(provider);

        Assert.Equal("alpha:hi", mediator.Send(new Exercises.Web.Ex007.Alpha.Echo("hi")));
        Assert.Equal("beta:hi", mediator.Send(new Exercises.Web.Ex007.Beta.Echo("hi")));
    }
}
