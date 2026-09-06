using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex065_CustomIoCDelegatesTests : CaliburnCoreContext
{
    [Fact]
    public void Unregistered_Type_Resolves_To_Null_Unlike_The_Harnesss_Own_Activator_Fallback()
    {
        new Ex065_CustomIoCDelegates().Install(new Ex065_MiniContainer());

        // Under the HARNESS's own delegates this would return a freshly Activator-created
        // instance instead - only true once THIS exercise's own delegates are installed.
        Assert.Null(IoC.Get<Ex065_UnregisteredThing>());
    }

    [Fact]
    public void Registering_An_Instance_Makes_IoC_Get_Return_The_Exact_Same_Instance()
    {
        var container = new Ex065_MiniContainer();
        var english = new Ex065_EnglishGreeter();
        container.Register(typeof(Ex065_IGreeter), english);
        new Ex065_CustomIoCDelegates().Install(container);

        Assert.Same(english, IoC.Get<Ex065_IGreeter>());
    }

    [Fact]
    public void GetAllInstances_Returns_Every_Registered_Instance_Assignable_To_The_Requested_Type()
    {
        var container = new Ex065_MiniContainer();
        var english = new Ex065_EnglishGreeter();
        var french = new Ex065_FrenchGreeter();
        var unrelated = new Ex065_UnregisteredThing();
        container.Register(typeof(Ex065_EnglishGreeter), english);
        container.Register(typeof(Ex065_FrenchGreeter), french);
        container.Register(typeof(Ex065_UnregisteredThing), unrelated);
        new Ex065_CustomIoCDelegates().Install(container);

        var all = IoC.GetAll<Ex065_IGreeter>().ToList();

        // A stub that only matches EXACT type equality (not assignability) would miss both -
        // neither registration key IS Ex065_IGreeter, both merely implement it. Pinning the
        // count to exactly 2 also rules out "unrelated" (registered under its own, unrelated
        // type, and structurally incapable of implementing Ex065_IGreeter at all) sneaking in -
        // there would be no room left for a third match.
        Assert.Equal(2, all.Count);
        Assert.Contains(english, all);
        Assert.Contains(french, all);
    }

    [Fact]
    public void BuildUp_Reaches_The_Installed_Containers_Own_BuildUp_Every_Time_Its_Called()
    {
        var container = new Ex065_MiniContainer();
        new Ex065_CustomIoCDelegates().Install(container);

        IoC.BuildUp(new object());
        Assert.Equal(1, container.BuildUpCallCount);

        IoC.BuildUp(new object());
        Assert.Equal(2, container.BuildUpCallCount);
    }

    [Fact]
    public void Installing_A_Second_Container_Replaces_The_First_Entirely_Rather_Than_Combining_With_It()
    {
        var containerA = new Ex065_MiniContainer();
        var english = new Ex065_EnglishGreeter();
        containerA.Register(typeof(Ex065_IGreeter), english);
        new Ex065_CustomIoCDelegates().Install(containerA);
        Assert.Same(english, IoC.Get<Ex065_IGreeter>());

        // A fresh, empty container - installing it must fully replace containerA's delegates,
        // not merely add to them.
        new Ex065_CustomIoCDelegates().Install(new Ex065_MiniContainer());

        Assert.Null(IoC.Get<Ex065_IGreeter>());
    }
}
