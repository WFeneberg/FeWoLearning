using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Expert;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex095_TrimmingFriendlyBindingsTests
{
    [AvaloniaFact]
    public void A_Registered_View_Model_Resolves_Through_Its_Factory()
    {
        var view = Ex095_TrimmingFriendlyBindings.BuildRegistryLocator()
            .ResolveView(new Ex095_ReportViewModel());

        var report = Assert.IsType<Ex095_ReportView>(view);
        Assert.Equal("registered", report.Origin);
    }

    [AvaloniaFact]
    public void The_Framework_Locator_Assigns_The_View_Model_For_You()
    {
        var viewModel = new Ex095_ReportViewModel();

        var view = Ex095_TrimmingFriendlyBindings.BuildRegistryLocator().ResolveView(viewModel);

        Assert.Same(viewModel, view!.ViewModel);
    }

    [AvaloniaFact]
    public void Each_Resolve_Calls_The_Factory_Again()
    {
        var locator = Ex095_TrimmingFriendlyBindings.BuildRegistryLocator();

        Assert.NotSame(
            locator.ResolveView(new Ex095_ReportViewModel()),
            locator.ResolveView(new Ex095_ReportViewModel()));
    }

    // The first trim-safety consequence, made visible - and it is worse than
    // returning nothing. Measured: Activator.CreateInstance THROWS
    // MissingMethodException for a view without a parameterless constructor, so a
    // name-based locator does not degrade, it crashes on the very view model the
    // registry resolves happily.
    [AvaloniaFact]
    public void The_Reflection_Locator_Crashes_On_A_View_That_Takes_Arguments()
    {
        Assert.Throws<System.MissingMethodException>(
            () => new Ex095_ReflectionLocator().ResolveView(new Ex095_ReportViewModel()));

        Assert.NotNull(Ex095_TrimmingFriendlyBindings.BuildRegistryLocator()
            .ResolveView(new Ex095_ReportViewModel()));
    }

    // The second, and the sharper one: the reflection locator resolves a view
    // model nobody ever registered, purely because a matching type happens to
    // exist. That is exactly the coupling a trimmer cannot see - so it is exactly
    // what a trim-safe registry must NOT do.
    [AvaloniaFact]
    public void Only_The_Reflection_Locator_Resolves_What_Nobody_Registered()
    {
        Assert.IsType<Ex095_LegacyView>(
            new Ex095_ReflectionLocator().ResolveView(new Ex095_LegacyViewModel()));

        Assert.Null(Ex095_TrimmingFriendlyBindings.BuildRegistryLocator()
            .ResolveView(new Ex095_LegacyViewModel()));
    }

    [AvaloniaFact]
    public void A_Null_View_Model_Resolves_To_Null_Either_Way()
    {
        Assert.Null(Ex095_TrimmingFriendlyBindings.BuildRegistryLocator().ResolveView(null));
        Assert.Null(new Ex095_ReflectionLocator().ResolveView(null));
    }
}
