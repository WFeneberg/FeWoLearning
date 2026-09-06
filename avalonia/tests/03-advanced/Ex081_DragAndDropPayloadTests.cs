using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex081_DragAndDropPayloadTests
{
    private static readonly Point Centre = new(100, 100);

    private static (Ex081_DragAndDropPayload Target, Window Window) Shown()
    {
        var target = new Ex081_DragAndDropPayload { Width = 60, Height = 40 };
        var window = ViewHarness.ShowWindow(target, 200, 200);
        return (target, window);
    }

    private static DataTransfer WithText(string text)
    {
        var transfer = new DataTransfer();
        transfer.Add(DataTransferItem.CreateText(text));
        return transfer;
    }

    // A payload carrying something this control cannot use: a string under an
    // application-private format rather than under DataFormat.Text.
    private static DataTransfer WithoutText()
    {
        var transfer = new DataTransfer();
        transfer.Add(DataTransferItem.Create(
            DataFormat.CreateStringApplicationFormat("fewolearning-unusable"), "opaque"));
        return transfer;
    }

    private static void Send(Window window, RawDragEventType type, DataTransfer transfer) =>
        window.DragDrop(Centre, type, transfer, DragDropEffects.Copy, RawInputModifiers.None);

    private static void FullDrag(Window window, DataTransfer transfer)
    {
        Send(window, RawDragEventType.DragEnter, transfer);
        Send(window, RawDragEventType.DragOver, transfer);
        Send(window, RawDragEventType.Drop, transfer);
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaFact]
    public void A_Text_Payload_Arrives_Through_All_Three_Events()
    {
        var (target, window) = Shown();

        FullDrag(window, WithText("payload"));

        Assert.Equal(["enter", "over", "drop:payload"], target.Log);
    }

    // The opt-in half, and it is not advisory: measured, a control without
    // AllowDrop receives no drag events at all, so this is what a solution that
    // wires the handlers but forgets SetAllowDrop fails on.
    [AvaloniaFact]
    public void The_Control_Opted_In_To_Drops()
    {
        var (target, _) = Shown();

        Assert.True(DragDrop.GetAllowDrop(target));
    }

    // DragOver is where a drop target answers "may I?", and answering with the
    // right effect is what the user sees as a cursor. Copy for something usable...
    [AvaloniaFact]
    public void A_Usable_Payload_Is_Offered_A_Copy_Effect()
    {
        var (target, window) = Shown();

        Send(window, RawDragEventType.DragEnter, WithText("payload"));
        Send(window, RawDragEventType.DragOver, WithText("payload"));
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(DragDropEffects.Copy, target.LastOfferedEffect);
    }

    // ...and None for something it cannot read. An implementation that offers Copy
    // unconditionally passes every other test in this file and fails here, which
    // is the whole point of separating the offer from the drop.
    [AvaloniaFact]
    public void An_Unusable_Payload_Is_Refused_In_DragOver()
    {
        var (target, window) = Shown();

        Send(window, RawDragEventType.DragEnter, WithoutText());
        Send(window, RawDragEventType.DragOver, WithoutText());
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(DragDropEffects.None, target.LastOfferedEffect);
    }

    [AvaloniaFact]
    public void A_Drop_Without_Text_Is_Recorded_As_Such_Rather_Than_Crashing()
    {
        var (target, window) = Shown();

        FullDrag(window, WithoutText());

        Assert.Equal(["enter", "over", "drop:none"], target.Log);
    }

    [AvaloniaFact]
    public void A_Second_Drag_Reads_Its_Own_Payload()
    {
        var (target, window) = Shown();

        FullDrag(window, WithText("first"));
        FullDrag(window, WithText("second"));

        Assert.Equal(["enter", "over", "drop:first", "enter", "over", "drop:second"], target.Log);
    }
}
