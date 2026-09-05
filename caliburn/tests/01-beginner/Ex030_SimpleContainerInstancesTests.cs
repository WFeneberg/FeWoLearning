using System.Collections.Generic;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex030_SimpleContainerInstancesTests : CaliburnCoreContext
{
    [Fact]
    public void RegisterInstance_Resolution_Is_The_Exact_Same_Object_Every_Time()
    {
        var subject = new Ex030_SimpleContainerInstances();
        var container = new SimpleContainer();
        var instance = new Ex030_Thing();
        subject.RegisterInstance<IEx030_Thing>(container, instance);

        var a = container.GetInstance(typeof(IEx030_Thing), null);
        var b = container.GetInstance(typeof(IEx030_Thing), null);

        Assert.Same(instance, a);
        Assert.Same(instance, b);
    }

    [Fact]
    public void RegisterHandler_Runs_The_Factory_Again_On_Every_Resolution()
    {
        var subject = new Ex030_SimpleContainerInstances();
        var container = new SimpleContainer();
        var callCount = 0;
        subject.RegisterHandler<IEx030_Thing>(container, _ => { callCount++; return new Ex030_Thing(); });

        container.GetInstance(typeof(IEx030_Thing), null);
        container.GetInstance(typeof(IEx030_Thing), null);

        Assert.Equal(2, callCount);
    }

    [Fact]
    public void RegisterHandler_Returns_Whatever_The_Factory_Produced_That_Call()
    {
        var subject = new Ex030_SimpleContainerInstances();
        var container = new SimpleContainer();
        var first = new Ex030_Thing();
        var second = new Ex030_Thing();
        var results = new Queue<IEx030_Thing>(new[] { first, second });
        subject.RegisterHandler<IEx030_Thing>(container, _ => results.Dequeue());

        var a = container.GetInstance(typeof(IEx030_Thing), null);
        var b = container.GetInstance(typeof(IEx030_Thing), null);

        Assert.Same(first, a);
        Assert.Same(second, b);
    }

    [Fact]
    public void CountRegistrations_Counts_Every_Registration_For_A_Service_Including_Duplicates()
    {
        var subject = new Ex030_SimpleContainerInstances();
        var container = new SimpleContainer();
        container.RegisterPerRequest(typeof(IEx030_Thing), null, typeof(Ex030_Thing));
        container.RegisterPerRequest(typeof(IEx030_Thing), null, typeof(Ex030_Thing));

        Assert.Equal(2, subject.CountRegistrations<IEx030_Thing>(container));
    }

    [Fact]
    public void CountRegistrations_For_A_Never_Registered_Service_Is_Zero()
    {
        var subject = new Ex030_SimpleContainerInstances();
        var container = new SimpleContainer();

        Assert.Equal(0, subject.CountRegistrations<IEx030_Thing>(container));
    }

    [Fact]
    public void RegisterInstance_Then_RegisterHandler_For_The_Same_Service_Counts_As_Two_Registrations()
    {
        // Proves GetAllInstances counts across DIFFERENT registration kinds, not just repeats of
        // the same kind - a wrong CountRegistrations that only tallies one style would fail here.
        var subject = new Ex030_SimpleContainerInstances();
        var container = new SimpleContainer();
        subject.RegisterInstance<IEx030_Thing>(container, new Ex030_Thing());
        subject.RegisterHandler<IEx030_Thing>(container, _ => new Ex030_Thing());

        Assert.Equal(2, subject.CountRegistrations<IEx030_Thing>(container));
    }
}
