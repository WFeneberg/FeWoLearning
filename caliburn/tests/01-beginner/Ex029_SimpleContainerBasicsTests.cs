using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex029_SimpleContainerBasicsTests : CaliburnCoreContext
{
    [Fact]
    public void Singleton_Resolves_The_Same_Instance_Every_Time()
    {
        var subject = new Ex029_SimpleContainerBasics();
        var container = new SimpleContainer();
        subject.RegisterSingleton<IEx029_Thing, Ex029_Thing>(container);

        var a = subject.Resolve<IEx029_Thing>(container);
        var b = subject.Resolve<IEx029_Thing>(container);

        Assert.NotNull(a);
        Assert.Same(a, b);
    }

    [Fact]
    public void PerRequest_Resolves_A_Fresh_Instance_Every_Time()
    {
        var subject = new Ex029_SimpleContainerBasics();
        var container = new SimpleContainer();
        subject.RegisterPerRequest<IEx029_Thing, Ex029_Thing>(container);

        var a = subject.Resolve<IEx029_Thing>(container);
        var b = subject.Resolve<IEx029_Thing>(container);

        Assert.NotNull(a);
        Assert.NotNull(b);
        Assert.NotSame(a, b);
    }

    [Fact]
    public void Resolving_A_Never_Registered_Service_Returns_Null_Not_A_Throw()
    {
        var subject = new Ex029_SimpleContainerBasics();
        var container = new SimpleContainer();

        var ex = Record.Exception(() => subject.Resolve<IEx029_Thing>(container));

        Assert.Null(ex);
        Assert.Null(subject.Resolve<IEx029_Thing>(container));
    }

    [Fact]
    public void A_Registered_Consumer_Gets_The_Registered_Dependency_Constructor_Injected()
    {
        var subject = new Ex029_SimpleContainerBasics();
        var container = new SimpleContainer();
        subject.RegisterSingleton<IEx029_Thing, Ex029_Thing>(container);
        subject.RegisterPerRequest<Ex029_ThingConsumer, Ex029_ThingConsumer>(container);

        var thing = subject.Resolve<IEx029_Thing>(container);
        var consumer = subject.Resolve<Ex029_ThingConsumer>(container);

        Assert.NotNull(consumer);
        Assert.Same(thing, consumer!.Thing);
    }

    [Fact]
    public void An_Unregistered_Consumer_Type_Resolves_To_Null_Even_Though_Its_Dependency_Is_Registered()
    {
        // The narrow, easy-to-assume-wrong part of constructor injection: registering IThing
        // alone does NOT make SimpleContainer able to reflectively construct an unregistered
        // consumer type on demand - only a type that is ITSELF registered gets built this way.
        var subject = new Ex029_SimpleContainerBasics();
        var container = new SimpleContainer();
        subject.RegisterSingleton<IEx029_Thing, Ex029_Thing>(container);

        Assert.Null(subject.Resolve<Ex029_ThingConsumer>(container));
    }

    [Fact]
    public void Two_Separate_Containers_Never_Share_Registrations()
    {
        var subject = new Ex029_SimpleContainerBasics();
        var containerA = new SimpleContainer();
        var containerB = new SimpleContainer();
        subject.RegisterSingleton<IEx029_Thing, Ex029_Thing>(containerA);

        Assert.NotNull(subject.Resolve<IEx029_Thing>(containerA));
        Assert.Null(subject.Resolve<IEx029_Thing>(containerB));
    }
}
