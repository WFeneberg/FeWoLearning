using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Expert;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex096_MultiWindowLifetimeTests
{
    private static Ex096_MultiWindowLifetime Opened()
    {
        var subject = new Ex096_MultiWindowLifetime();
        subject.Shell.Show();
        Dispatcher.UIThread.RunJobs();
        subject.Open();
        Dispatcher.UIThread.RunJobs();
        return subject;
    }

    // Stated as a test rather than only in the stub, because it is the reason this
    // row does not use the API its name suggests: there is no lifetime here at
    // all, so any code reading it must cope with null. The window half is asserted
    // alongside it on purpose - both to keep this red against the untouched stub,
    // and because the interesting claim is the conjunction: no lifetime, and
    // multiple windows work anyway.
    [AvaloniaFact]
    public void Windows_Work_Even_Though_There_Is_No_Desktop_Lifetime()
    {
        Assert.Null(Application.Current?.ApplicationLifetime);
        Assert.False(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime);

        var subject = Opened();

        Assert.NotNull(subject.Tool);
        Assert.True(subject.Tool!.IsVisible);
    }

    [AvaloniaFact]
    public void The_Tool_Window_Is_Owned_By_The_Shell()
    {
        var subject = Opened();

        Assert.NotNull(subject.Tool);
        Assert.Same(subject.Shell, subject.Tool!.Owner);
        Assert.Single(subject.Shell.OwnedWindows);
        Assert.True(subject.Tool.IsVisible);
    }

    // Show() without an owner would leave OwnedWindows empty while every other
    // assertion about visibility still passed, which is why ownership is checked
    // on both sides.
    [AvaloniaFact]
    public void The_Shell_Knows_About_Its_Child()
    {
        var subject = Opened();

        Assert.Contains(subject.Tool!, subject.Shell.OwnedWindows);
    }

    // The refusal, and it is not cosmetic: the window is still there and still
    // owned afterwards. A handler that merely logs and lets the close proceed
    // fails on the visibility.
    [AvaloniaFact]
    public void An_Unconfirmed_Close_Is_Refused()
    {
        var subject = Opened();

        subject.RequestClose();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(["attempt"], subject.CloseAttempts);
        Assert.True(subject.Tool!.IsVisible);
        Assert.Single(subject.Shell.OwnedWindows);
    }

    [AvaloniaFact]
    public void Refusing_Twice_Records_Both_Attempts()
    {
        var subject = Opened();

        subject.RequestClose();
        subject.RequestClose();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(["attempt", "attempt"], subject.CloseAttempts);
        Assert.True(subject.Tool!.IsVisible);
    }

    [AvaloniaFact]
    public void A_Confirmed_Close_Goes_Through_And_Releases_The_Ownership()
    {
        var subject = Opened();

        subject.RequestClose();
        Dispatcher.UIThread.RunJobs();
        subject.Confirmed = true;
        subject.RequestClose();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(["attempt", "attempt"], subject.CloseAttempts);
        Assert.False(subject.Tool!.IsVisible);
        Assert.Empty(subject.Shell.OwnedWindows);
    }

    // Confirming up front means the first attempt already succeeds, so the
    // handler really is consulting Confirmed rather than counting attempts.
    [AvaloniaFact]
    public void Confirming_First_Closes_On_The_First_Attempt()
    {
        var subject = new Ex096_MultiWindowLifetime { Confirmed = true };
        subject.Shell.Show();
        Dispatcher.UIThread.RunJobs();
        subject.Open();
        Dispatcher.UIThread.RunJobs();

        subject.RequestClose();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(["attempt"], subject.CloseAttempts);
        Assert.False(subject.Tool!.IsVisible);
        Assert.Empty(subject.Shell.OwnedWindows);
    }
}
