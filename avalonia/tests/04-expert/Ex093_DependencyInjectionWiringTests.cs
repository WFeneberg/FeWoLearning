using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Expert;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex093_DependencyInjectionWiringTests
{
    private static Ex093_DependencyInjectionWiring Wired()
    {
        var subject = new Ex093_DependencyInjectionWiring();
        subject.Wire();
        return subject;
    }

    // Transient: a fresh instance every time, and the factory really does run
    // again - the build counter is what separates "a new object" from "a cached
    // object handed out twice".
    [AvaloniaFact]
    public void A_Transient_Registration_Builds_Every_Time()
    {
        var subject = Wired();

        var first = subject.Resolve<Ex093_Clock>();
        var second = subject.Resolve<Ex093_Clock>();

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.NotSame(first, second);
        Assert.Equal(1, first!.Build);
        Assert.Equal(2, second!.Build);
        Assert.Equal(2, subject.TransientBuilds);
    }

    // Nothing is built until it is asked for, which is the whole difference
    // between a lazy singleton and a constant.
    [AvaloniaFact]
    public void A_Lazy_Singleton_Builds_Once_And_Not_Before_It_Is_Needed()
    {
        var subject = Wired();

        Assert.Equal(0, subject.SingletonBuilds);

        var first = subject.Resolve<Ex093_Cache>();
        var second = subject.Resolve<Ex093_Cache>();

        Assert.Same(first, second);
        Assert.Equal(1, subject.SingletonBuilds);
    }

    [AvaloniaFact]
    public void A_Constant_Is_The_Same_Instance_Too()
    {
        var subject = Wired();

        var first = subject.Resolve<Ex093_Settings>();
        var second = subject.Resolve<Ex093_Settings>();

        Assert.NotNull(first);
        Assert.Same(first, second);
    }

    // An unregistered type is a null, not a throw - so a container miss is
    // something callers have to check for rather than something that announces
    // itself.
    [AvaloniaFact]
    public void An_Unregistered_Type_Resolves_To_Null()
    {
        var subject = Wired();

        Assert.Null(subject.Resolve<Ex093_Missing>());
    }

    // The registrations have to live in the resolver rather than in a private
    // dictionary behind Resolve: this asks the container directly, bypassing the
    // exercise's own accessor entirely.
    [AvaloniaFact]
    public void The_Registrations_Are_In_The_Resolver_Itself()
    {
        var subject = Wired();

        Assert.True(subject.Resolver.HasRegistration(typeof(Ex093_Clock)));
        Assert.True(subject.Resolver.HasRegistration(typeof(Ex093_Cache)));
        Assert.True(subject.Resolver.HasRegistration(typeof(Ex093_Settings)));
        Assert.False(subject.Resolver.HasRegistration(typeof(Ex093_Missing)));
    }

    // Two wirings are two containers: the resolver is an instance, not global
    // state, which is what keeps this exercise from leaking into the next test.
    [AvaloniaFact]
    public void Each_Instance_Has_Its_Own_Container()
    {
        var first = Wired();
        var second = Wired();

        Assert.NotSame(first.Resolver, second.Resolver);
        Assert.Same(first.Resolve<Ex093_Cache>(), first.Resolve<Ex093_Cache>());
        Assert.NotSame(first.Resolve<Ex093_Cache>(), second.Resolve<Ex093_Cache>());
    }
}
