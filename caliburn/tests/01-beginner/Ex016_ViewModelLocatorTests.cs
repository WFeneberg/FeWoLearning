using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex016_ViewModelLocatorTests : CaliburnViewContext
{
    [WpfFact]
    public void LocateViewModelType_Resolves_ProbeView_To_ProbeViewModel()
    {
        var subject = new Ex016_ViewModelLocator();

        var type = subject.LocateViewModelType(typeof(Ex016_ProbeView));

        Assert.Equal(typeof(Ex016_ProbeViewModel), type);
    }

    [WpfFact]
    public void LocateViewModelType_Resolves_A_Second_Unrelated_View_To_Its_Own_ViewModel_Not_A_Hardcoded_One()
    {
        var subject = new Ex016_ViewModelLocator();

        Assert.Equal(typeof(Ex016_ProbeViewModel), subject.LocateViewModelType(typeof(Ex016_ProbeView)));
        Assert.Equal(typeof(Ex016_SecondViewModel), subject.LocateViewModelType(typeof(Ex016_SecondView)));
    }

    [WpfFact]
    public void LocateViewModel_Constructs_A_Fresh_Instance_Each_Call_When_Nothing_Is_Registered()
    {
        var subject = new Ex016_ViewModelLocator();

        var first = subject.LocateViewModel(new Ex016_ProbeView());
        var second = subject.LocateViewModel(new Ex016_ProbeView());

        Assert.IsType<Ex016_ProbeViewModel>(first);
        Assert.NotSame(first, second);
    }

    [WpfFact]
    public void LocateViewModel_Returns_The_Instance_Registered_In_The_Container_Not_A_Fresh_One()
    {
        var subject = new Ex016_ViewModelLocator();
        var registered = new Ex016_ProbeViewModel();
        Container.RegisterInstance(typeof(Ex016_ProbeViewModel), null, registered);

        var located = subject.LocateViewModel(new Ex016_ProbeView());

        // A wrapper that hand-rolls "Activator.CreateInstance(LocateViewModelType(...))"
        // instead of delegating to ViewModelLocator.LocateForView would never see this
        // registration - it would fabricate a brand-new instance instead.
        Assert.Same(registered, located);
    }

    [WpfFact]
    public void LocateViewModel_For_A_View_With_No_Matching_ViewModel_Returns_Null_Not_A_Placeholder()
    {
        var subject = new Ex016_ViewModelLocator();

        // Unlike ex013's ViewLocator, which hands back a placeholder TextBlock for a
        // model with no view, LocateForView on this side is a plain null.
        var located = subject.LocateViewModel(new Ex016_OrphanView());

        Assert.Null(located);
    }

    [WpfFact]
    public void ViewModelLocator_Keeps_Its_Own_NameTransformer_A_Different_Object_From_ViewLocators()
    {
        // A structural assertion about static state alone would pass even against an
        // untouched stub - drive the throwing member too, so this fact is only ever
        // green once LocateViewModelType is actually implemented.
        var subject = new Ex016_ViewModelLocator();
        Assert.Equal(typeof(Ex016_ProbeViewModel), subject.LocateViewModelType(typeof(Ex016_ProbeView)));

        Assert.NotSame(ViewLocator.NameTransformer, ViewModelLocator.NameTransformer);
        Assert.Equal(4, ViewModelLocator.NameTransformer.Count);
        Assert.Equal(4, ViewLocator.NameTransformer.Count);
    }
}
