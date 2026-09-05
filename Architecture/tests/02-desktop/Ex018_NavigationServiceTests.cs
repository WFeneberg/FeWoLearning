using FeWoLearning.Architecture.Exercises.Desktop.Ex018;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex018_NavigationServiceTests
{
    [Fact]
    public void The_First_Navigation_Shows_A_Page_With_Nothing_Behind_It()
    {
        var navigation = new NavigationService();
        var home = new PageViewModel("home");

        navigation.NavigateTo(home);

        Assert.Same(home, navigation.Current);
        Assert.False(navigation.CanGoBack);
        Assert.Equal(["to"], home.Lifecycle);
    }

    [Fact]
    public void A_Second_Navigation_Puts_The_First_Page_Behind_It()
    {
        var navigation = new NavigationService();
        var home = new PageViewModel("home");
        var details = new PageViewModel("details");

        navigation.NavigateTo(home);
        navigation.NavigateTo(details);

        Assert.Same(details, navigation.Current);
        Assert.True(navigation.CanGoBack);
        Assert.Equal(["to", "from"], home.Lifecycle);
        Assert.Equal(["to"], details.Lifecycle);
    }

    [Fact]
    public void Mechanism_Going_Back_Restores_The_Same_Instance()
    {
        // The clause that matters. Going back is not navigating forward to a page that
        // happens to look the same: the user expects their half-filled form, their
        // scroll position and their selection to still be there. A service that
        // reconstructs the view model passes every other assertion in this file.
        var navigation = new NavigationService();
        var home = new PageViewModel("home");

        navigation.NavigateTo(home);
        navigation.NavigateTo(new PageViewModel("details"));
        navigation.GoBack();

        Assert.Same(home, navigation.Current);
    }

    [Fact]
    public void Going_Back_Runs_The_Lifecycle_On_Both_Pages_In_Order()
    {
        var navigation = new NavigationService();
        var home = new PageViewModel("home");
        var details = new PageViewModel("details");

        navigation.NavigateTo(home);
        navigation.NavigateTo(details);
        navigation.GoBack();

        Assert.Equal(["to", "from"], details.Lifecycle);
        // Home is entered twice, so it must be told twice - a page that caches "I am
        // already loaded" from a single OnNavigatedTo never refreshes on return.
        Assert.Equal(["to", "from", "to"], home.Lifecycle);
        Assert.False(navigation.CanGoBack);
    }

    [Fact]
    public void Going_Back_From_The_Root_Is_Refused()
    {
        var navigation = new NavigationService();
        navigation.NavigateTo(new PageViewModel("home"));

        Assert.Throws<InvalidOperationException>(navigation.GoBack);
    }
}
