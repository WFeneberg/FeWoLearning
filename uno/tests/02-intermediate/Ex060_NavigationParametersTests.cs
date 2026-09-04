using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex060_NavigationParametersTests : UnoTestContext
{
    private static Frame Frame()
    {
        Ex060_DetailPage.Received.Clear();
        Ex060_DetailPage.Rejected = 0;
        return Layout(Ex060_NavigationParameters.CreateFrame());
    }

    private static Ex060_DetailRequest Request(int id = 7, string title = "Invoice") => new(id, title);

    [Fact]
    public void The_Request_Reaches_The_Page()
    {
        var frame = Frame();
        var request = Request();

        Assert.True(Ex060_NavigationParameters.OpenDetail(frame, request));
        Assert.Equal([request], Ex060_DetailPage.Received);
    }

    [Fact]
    public void The_Page_Shows_The_Requested_Title()
    {
        var frame = Frame();

        Ex060_NavigationParameters.OpenDetail(frame, Request(title: "Invoice"));

        Assert.Equal("Invoice", FindDescendant<TextBlock>(frame).Text);
    }

    [Fact]
    public void A_Parameter_It_Cannot_Use_Is_Rejected_Rather_Than_Thrown()
    {
        var frame = Frame();

        frame.Navigate(typeof(Ex060_DetailPage), "just a string");

        // The parameter is typed as object, so this compiles at every call site. A page
        // that casts instead of checking fails at runtime for exactly one caller.
        Assert.Equal(1, Ex060_DetailPage.Rejected);
        Assert.Empty(Ex060_DetailPage.Received);
    }

    [Fact]
    public void No_Parameter_At_All_Is_Also_Rejected()
    {
        var frame = Frame();

        frame.Navigate(typeof(Ex060_DetailPage));

        Assert.Equal(1, Ex060_DetailPage.Rejected);
    }

    [Fact]
    public void Opening_Twice_Records_Both_Requests()
    {
        var frame = Frame();

        Ex060_NavigationParameters.OpenDetail(frame, Request(1, "First"));
        Ex060_NavigationParameters.OpenDetail(frame, Request(2, "Second"));

        Assert.Equal([Request(1, "First"), Request(2, "Second")], Ex060_DetailPage.Received);
    }

    [Fact]
    public void Going_Back_Replays_The_Previous_Parameter()
    {
        var frame = Frame();
        Ex060_NavigationParameters.OpenDetail(frame, Request(1, "First"));
        Ex060_NavigationParameters.OpenDetail(frame, Request(2, "Second"));

        Assert.True(Ex060_NavigationParameters.GoBackIfPossible(frame));

        // A back stack entry stores the parameter, not the page instance: the page is built
        // again and told the old request.
        Assert.Equal(Request(1, "First"), Ex060_DetailPage.Received[^1]);
    }

    [Fact]
    public void Going_Back_With_An_Empty_Stack_Does_Nothing()
    {
        var frame = Frame();
        Ex060_NavigationParameters.OpenDetail(frame, Request());

        Assert.False(Ex060_NavigationParameters.GoBackIfPossible(frame));
        Assert.Single(Ex060_DetailPage.Received);
    }

    [Fact]
    public void A_Record_Parameter_Survives_Whole()
    {
        var frame = Frame();

        Ex060_NavigationParameters.OpenDetail(frame, Request(42, "Answer"));

        // Nothing is serialised in-process, so a typed parameter is a better contract than
        // a string that has to be parsed on arrival.
        var received = Ex060_DetailPage.Received[0];
        Assert.Equal(42, received.Id);
        Assert.Equal("Answer", received.Title);
    }
}
