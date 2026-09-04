using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex059_FrameNavigationTests : UnoTestContext
{
    private static Frame Frame()
    {
        Ex059_FirstPage.Arrivals = 0;
        Ex059_FirstPage.Departures = 0;
        Ex059_SecondPage.Arrivals = 0;
        return Layout(Ex059_FrameNavigation.CreateFrameOnFirstPage());
    }

    [Fact]
    public void The_Frame_Opens_On_The_First_Page()
    {
        var frame = Frame();

        Assert.IsType<Ex059_FirstPage>(frame.Content);
        Assert.Equal(1, Ex059_FirstPage.Arrivals);
    }

    [Fact]
    public void The_First_Page_Renders_Its_Content()
    {
        var frame = Frame();

        // The Frame instantiates the type and nothing else - a page that builds no content
        // shows an empty frame with no error.
        Assert.Equal("first", FindDescendant<TextBlock>(frame).Text);
    }

    [Fact]
    public void A_Fresh_Frame_Has_Nowhere_To_Go_Back_To()
    {
        var frame = Frame();

        Assert.False(frame.CanGoBack);
        Assert.Equal(0, frame.BackStackDepth);
    }

    [Fact]
    public void Navigating_On_Pushes_The_Back_Stack()
    {
        var frame = Frame();

        frame.Navigate(typeof(Ex059_SecondPage));

        Assert.IsType<Ex059_SecondPage>(frame.Content);
        Assert.True(frame.CanGoBack);
        Assert.Equal(1, frame.BackStackDepth);
    }

    [Fact]
    public void Leaving_A_Page_Tells_It_So()
    {
        var frame = Frame();

        frame.Navigate(typeof(Ex059_SecondPage));

        Assert.Equal(1, Ex059_FirstPage.Departures);
    }

    [Fact]
    public void Going_Back_Returns_To_The_First_Page()
    {
        var frame = Frame();
        frame.Navigate(typeof(Ex059_SecondPage));

        frame.GoBack();

        Assert.IsType<Ex059_FirstPage>(frame.Content);
        Assert.Equal(0, frame.BackStackDepth);
    }

    [Fact]
    public void Going_Back_Arrives_Again()
    {
        var frame = Frame();
        frame.Navigate(typeof(Ex059_SecondPage));

        frame.GoBack();

        // A back navigation is a navigation: OnNavigatedTo runs again, which is why it is
        // the right place to refresh what a page shows.
        Assert.Equal(2, Ex059_FirstPage.Arrivals);
    }

    [Fact]
    public void The_Second_Page_Is_Reached_Once_Per_Navigation()
    {
        var frame = Frame();

        frame.Navigate(typeof(Ex059_SecondPage));
        frame.GoBack();
        frame.Navigate(typeof(Ex059_SecondPage));

        Assert.Equal(2, Ex059_SecondPage.Arrivals);
    }
}
