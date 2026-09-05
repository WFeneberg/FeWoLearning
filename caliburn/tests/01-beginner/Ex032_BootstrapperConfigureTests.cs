using System.Linq;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex032_BootstrapperConfigureTests : CaliburnCoreContext
{
    [Fact]
    public void Initialize_Runs_Configure_And_Registers_The_Greeter()
    {
        var boot = new Ex032_BootstrapperConfigure();
        // Nothing runs Configure() just by constructing the bootstrapper.
        Assert.Null(boot.Container.GetInstance(typeof(IEx032_Greeter), null));

        boot.Initialize();

        var greeter = boot.Container.GetInstance(typeof(IEx032_Greeter), null);
        Assert.NotNull(greeter);
        Assert.IsType<Ex032_Greeter>(greeter);
    }

    [Fact]
    public void Initialize_Installs_The_Bootstrapper_Overrides_Behind_The_Global_IoC_Facade()
    {
        var boot = new Ex032_BootstrapperConfigure();

        boot.Initialize();
        var greeter = IoC.Get<IEx032_Greeter>();

        Assert.NotNull(greeter);
        Assert.Same(boot.Container.GetInstance(typeof(IEx032_Greeter), null), greeter);
    }

    [Fact]
    public void GetAllInstances_Override_Surfaces_Every_Registration_Through_IoC_GetAll()
    {
        var boot = new Ex032_BootstrapperConfigure();
        boot.Initialize();
        // A second registration for the same service, added directly - proves GetAllInstances
        // is not just returning a single fixed answer.
        boot.Container.RegisterPerRequest(typeof(IEx032_Greeter), null, typeof(Ex032_Greeter));

        var all = IoC.GetAll<IEx032_Greeter>().ToList();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public void BuildUp_Override_Runs_Property_Injection_Through_IoC_BuildUp()
    {
        var boot = new Ex032_BootstrapperConfigure();
        boot.Initialize();
        var consumer = new Ex032_GreeterConsumer();

        IoC.BuildUp(consumer);

        Assert.NotNull(consumer.Greeter);
        Assert.Same(boot.Container.GetInstance(typeof(IEx032_Greeter), null), consumer.Greeter);
    }

    [Fact]
    public void Initialize_Is_Idempotent_Configure_Runs_Only_Once()
    {
        var boot = new Ex032_BootstrapperConfigure();

        boot.Initialize();
        boot.Initialize();
        boot.Initialize();

        // Not a learner choice - Initialize() itself is BootstrapperBase's, not overridden here -
        // but worth knowing: a real app calls Initialize() from OnStartup, and a second call
        // (however that might happen) must not re-run Configure(). ConfigureCallCount is what
        // the learner's own Configure() increments, so this still exercises their code.
        Assert.Equal(1, boot.ConfigureCallCount);
    }
}
