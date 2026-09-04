using FeWoLearning.Uno.Exercises.Expert;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex094_NavigationServiceTests : UnoTestContext
{
    /// <summary>The fake the interface exists for: no Frame, no UI, no layout.</summary>
    private sealed class RecordingNavigator : IEx094_Navigator
    {
        public List<(string Route, object? Parameter)> Navigations { get; } = [];

        public int BackCount { get; private set; }

        public bool CanGoBack { get; set; }

        public bool Navigate(string route, object? parameter = null)
        {
            Navigations.Add((route, parameter));
            return true;
        }

        public bool GoBack()
        {
            BackCount++;
            return CanGoBack;
        }
    }

    private static Ex094_FrameNavigator Navigator(out Frame frame)
    {
        Ex094_DetailPage.Received.Clear();
        Ex094_HomePage.Arrivals = 0;
        frame = Layout(new Frame());

        return new Ex094_FrameNavigator(
            frame,
            new Dictionary<string, Type>
            {
                ["home"] = typeof(Ex094_HomePage),
                ["detail"] = typeof(Ex094_DetailPage),
            });
    }

    [Fact]
    public void A_Known_Route_Navigates()
    {
        var navigator = Navigator(out var frame);

        Assert.True(navigator.Navigate("home"));
        Assert.IsType<Ex094_HomePage>(frame.Content);
    }

    [Fact]
    public void The_Route_Name_Is_Case_Insensitive()
    {
        var navigator = Navigator(out var frame);

        Assert.True(navigator.Navigate("HOME"));
        Assert.IsType<Ex094_HomePage>(frame.Content);
    }

    [Fact]
    public void An_Unknown_Route_Is_Refused_Rather_Than_Thrown()
    {
        var navigator = Navigator(out var frame);

        // A deep link or a stale button should not take the app down.
        Assert.False(navigator.Navigate("nowhere"));
        Assert.Null(frame.Content);
    }

    [Fact]
    public void A_Parameter_Reaches_The_Page()
    {
        var navigator = Navigator(out _);

        navigator.Navigate("detail", 42);

        Assert.Equal([42], Ex094_DetailPage.Received);
    }

    [Fact]
    public void Going_Back_Works_Through_The_Adapter()
    {
        var navigator = Navigator(out var frame);
        navigator.Navigate("home");
        navigator.Navigate("detail");

        Assert.True(navigator.CanGoBack);
        Assert.True(navigator.GoBack());
        Assert.IsType<Ex094_HomePage>(frame.Content);
    }

    [Fact]
    public void Going_Back_With_An_Empty_Stack_Is_Refused()
    {
        var navigator = Navigator(out _);
        navigator.Navigate("home");

        Assert.False(navigator.CanGoBack);
        Assert.False(navigator.GoBack());
    }

    [Fact]
    public void The_View_Model_Navigates_Without_A_Frame()
    {
        var navigator = new RecordingNavigator();
        var viewModel = new Ex094_MenuViewModel(navigator);

        Assert.True(viewModel.OpenDetail(42));

        // No Frame anywhere in this test: that is the entire reason the interface exists.
        Assert.Equal([("detail", (object?)42)], navigator.Navigations);
    }

    [Fact]
    public void The_View_Model_Goes_Back_Through_The_Navigator()
    {
        var navigator = new RecordingNavigator { CanGoBack = true };
        var viewModel = new Ex094_MenuViewModel(navigator);

        Assert.True(viewModel.Back());
        Assert.Equal(1, navigator.BackCount);
    }

    [Fact]
    public void The_View_Model_Reports_A_Refused_Back()
    {
        var navigator = new RecordingNavigator { CanGoBack = false };
        var viewModel = new Ex094_MenuViewModel(navigator);

        Assert.False(viewModel.Back());
    }

    [Fact]
    public void The_Same_View_Model_Works_Against_The_Real_Frame()
    {
        var navigator = Navigator(out var frame);
        var viewModel = new Ex094_MenuViewModel(navigator);

        viewModel.OpenDetail(7);

        // The point of the seam: the fake and the real adapter are interchangeable, so what
        // the tests above proved about the view model holds in the app.
        Assert.IsType<Ex094_DetailPage>(frame.Content);
        Assert.Equal([7], Ex094_DetailPage.Received);
    }
}
