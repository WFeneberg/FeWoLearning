using System.Linq;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex031_IoCFacadeTests : CaliburnCoreContext
{
    [Fact]
    public void Resolve_Returns_The_Registered_Singleton_Instance()
    {
        var subject = new Ex031_IoCFacade();
        Container.RegisterSingleton(typeof(IEx031_Thing), null, typeof(Ex031_Thing));

        var result = subject.Resolve<IEx031_Thing>();

        Assert.NotNull(result);
        Assert.Same(Container.GetInstance(typeof(IEx031_Thing), null), result);
    }

    [Fact]
    public void Resolve_Returns_A_Fresh_PerRequest_Instance_Every_Call()
    {
        var subject = new Ex031_IoCFacade();
        Container.RegisterPerRequest(typeof(IEx031_Thing), null, typeof(Ex031_Thing));

        var a = subject.Resolve<IEx031_Thing>();
        var b = subject.Resolve<IEx031_Thing>();

        Assert.NotNull(a);
        Assert.NotNull(b);
        // A hard-coded "resolve once, cache forever" implementation would return the same
        // instance twice - it does not, because every call goes through IoC.Get again.
        Assert.NotSame(a, b);
    }

    [Fact]
    public void ResolveAll_Returns_Empty_When_Nothing_Is_Registered()
    {
        var subject = new Ex031_IoCFacade();

        var result = subject.ResolveAll<IEx031_Thing>();

        Assert.Empty(result);
    }

    [Fact]
    public void ResolveAll_Counts_Every_Registration_Not_Just_The_Latest()
    {
        var subject = new Ex031_IoCFacade();
        Container.RegisterSingleton(typeof(IEx031_Thing), null, typeof(Ex031_Thing));
        Container.RegisterPerRequest(typeof(IEx031_Thing), null, typeof(Ex031_Thing));

        var all = subject.ResolveAll<IEx031_Thing>().ToList();

        // Two separate registrations for the same service - a naive "return the first match"
        // resolver would report 1, not 2.
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public void Inject_Populates_An_Interface_Typed_Property_But_Leaves_Unregistered_And_Concrete_Typed_Properties_Untouched()
    {
        var subject = new Ex031_IoCFacade();
        Container.RegisterSingleton(typeof(IEx031_Thing), null, typeof(Ex031_Thing));
        // Also register the CONCRETE type under its own name - proves the next assertion is
        // about interface-vs-concrete, not merely "nothing was registered for it".
        Container.RegisterSingleton(typeof(Ex031_Thing), null, typeof(Ex031_Thing));
        var consumer = new Ex031_PropertyConsumer();

        subject.Inject(consumer);

        Assert.NotNull(consumer.Thing);
        Assert.Same(Container.GetInstance(typeof(IEx031_Thing), null), consumer.Thing);
        // IEx031_Other was never registered - a real BuildUp leaves it alone rather than
        // throwing or forcing some default.
        Assert.Null(consumer.Other);
        // Measured: SimpleContainer.BuildUp only ever injects INTERFACE-typed properties - a
        // concrete-typed property is left alone even though its own concrete type IS registered.
        Assert.Null(consumer.ConcreteThing);
    }

    [Fact]
    public void Resolve_Distinguishes_Between_Two_Different_Registered_Services()
    {
        var subject = new Ex031_IoCFacade();
        Container.RegisterSingleton(typeof(IEx031_Thing), null, typeof(Ex031_Thing));
        Container.RegisterSingleton(typeof(IEx031_Other), null, typeof(Ex031_Other));

        var thing = subject.Resolve<IEx031_Thing>();
        var other = subject.Resolve<IEx031_Other>();

        Assert.IsType<Ex031_Thing>(thing);
        Assert.IsType<Ex031_Other>(other);
    }
}
