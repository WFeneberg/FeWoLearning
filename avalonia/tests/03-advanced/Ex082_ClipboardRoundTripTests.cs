using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex082_ClipboardRoundTripTests
{
    // The clipboard is process-global and this suite runs serially, so each test
    // starts by wiping whatever the last one left behind rather than trusting it
    // to be empty.
    private static async Task<Ex082_ClipboardRoundTrip> Shown()
    {
        var control = new Ex082_ClipboardRoundTrip { Width = 40, Height = 20 };
        ViewHarness.ShowWindow(control, 200, 120);
        await control.ClearAsync();
        return control;
    }

    // The clipboard hangs off the TopLevel, so a control that has never been in a
    // window has none - and every control starts that way. An implementation
    // reaching for a static or an Application-level clipboard cannot reproduce
    // this null.
    [AvaloniaFact]
    public void A_Control_Outside_A_Window_Has_No_Clipboard()
    {
        Assert.Null(new Ex082_ClipboardRoundTrip().Clipboard);
    }

    [AvaloniaFact]
    public async Task A_Shown_Control_Finds_Its_Windows_Clipboard()
    {
        var control = await Shown();

        Assert.NotNull(control.Clipboard);
    }

    [AvaloniaFact]
    public async Task Text_Survives_A_Round_Trip()
    {
        var control = await Shown();

        await control.CopyAsync("hello clipboard");

        Assert.Equal("hello clipboard", await control.PasteAsync());
    }

    [AvaloniaFact]
    public async Task A_Second_Copy_Replaces_The_First()
    {
        var control = await Shown();

        await control.CopyAsync("first");
        await control.CopyAsync("second");

        Assert.Equal("second", await control.PasteAsync());
    }

    // Measured: TryGetDataAsync returns null on an empty clipboard rather than an
    // empty transfer, so this is the case that throws in an implementation which
    // assumes something is always there - and an empty clipboard is the normal
    // state, not an edge case.
    [AvaloniaFact]
    public async Task An_Empty_Clipboard_Pastes_Null_Rather_Than_Throwing()
    {
        var control = await Shown();

        Assert.Null(await control.PasteAsync());
    }

    [AvaloniaFact]
    public async Task Clearing_Removes_What_Was_There()
    {
        var control = await Shown();

        await control.CopyAsync("transient");
        await control.ClearAsync();

        Assert.Null(await control.PasteAsync());
    }

    [AvaloniaFact]
    public async Task The_Payload_Is_Stored_Under_The_Text_Format()
    {
        var control = await Shown();

        await control.CopyAsync("typed payload");

        using var transfer = await control.Clipboard!.TryGetDataAsync();
        Assert.NotNull(transfer);
        Assert.Contains(DataFormat.Text, transfer!.Formats);
    }
}
