using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex064_DesignTimeDetectionTests : CaliburnViewContext
{
    [WpfFact]
    public void Under_The_Real_XamlPlatformProvider_Design_Mode_Is_False_So_The_ViewModel_Shows_The_Real_Greeting()
    {
        var vm = new Ex064_DesignTimeAwareViewModel { RealGreeting = "Hello, Ada" };

        Assert.False(Execute.InDesignMode);
        Assert.Equal("Hello, Ada", vm.Greeting);
    }

    [WpfFact]
    public void Swapping_To_The_Default_Platform_Provider_Reports_The_Surprising_True_And_The_ViewModel_Shows_Canned_Data()
    {
        // Cleaned up automatically - CaliburnViewContext resets PlatformProvider.Current per test.
        PlatformProvider.Current = new DefaultPlatformProvider();
        var vm = new Ex064_DesignTimeAwareViewModel { RealGreeting = "Hello, Ada" };

        Assert.True(Execute.InDesignMode);
        // The view model reads Execute.InDesignMode itself - it never had to change for this.
        Assert.Equal(Ex064_DesignTimeAwareViewModel.SampleGreeting, vm.Greeting);
    }

    [WpfFact]
    public void A_Custom_Provider_Reporting_Design_Mode_True_Also_Flips_Execute_And_The_ViewModel()
    {
        PlatformProvider.Current = new Ex064_FakeDesignModeProvider(new XamlPlatformProvider(), designMode: true);
        var vm = new Ex064_DesignTimeAwareViewModel { RealGreeting = "Hello, Grace" };

        Assert.True(Execute.InDesignMode);
        Assert.Equal(Ex064_DesignTimeAwareViewModel.SampleGreeting, vm.Greeting);
    }

    [WpfFact]
    public void The_Same_Custom_Provider_Type_Reporting_Design_Mode_False_Reads_The_Flag_Not_A_Hardcoded_True()
    {
        PlatformProvider.Current = new Ex064_FakeDesignModeProvider(new XamlPlatformProvider(), designMode: false);
        var vm = new Ex064_DesignTimeAwareViewModel { RealGreeting = "Hello, Grace" };

        // A stub that hardcodes InDesignMode => true (a plausible copy-paste from the test
        // above) passes the previous test and fails only here.
        Assert.False(Execute.InDesignMode);
        Assert.Equal("Hello, Grace", vm.Greeting);
    }

    [WpfFact]
    public void The_Custom_Providers_InDesignMode_Is_Independent_Of_What_The_Wrapped_Provider_Would_Say()
    {
        // Wraps a DefaultPlatformProvider, which on its own reports InDesignMode == true - the
        // fake still has to answer false here, proving it genuinely returns ITS OWN flag rather
        // than forwarding to inner.InDesignMode (a very plausible copy-paste mistake, since
        // every other member on this class IS a plain forward to inner).
        PlatformProvider.Current = new Ex064_FakeDesignModeProvider(new DefaultPlatformProvider(), designMode: false);
        var vm = new Ex064_DesignTimeAwareViewModel { RealGreeting = "Hello, Marie" };

        Assert.False(Execute.InDesignMode);
        Assert.Equal("Hello, Marie", vm.Greeting);
    }
}
