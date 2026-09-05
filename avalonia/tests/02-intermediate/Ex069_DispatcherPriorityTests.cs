using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex069_DispatcherPriorityTests
{
    // The first discriminator: Post queues, it does not invoke. Calling the four
    // actions directly - which would produce the right final log - fails right
    // here, before any draining has happened.
    [AvaloniaFact]
    public void Posting_Runs_Nothing_Yet()
    {
        var subject = new Ex069_DispatcherPriority();

        subject.PostAll();

        Assert.Empty(subject.Log);
    }

    // The second: RunJobs(Input) drains everything at Input priority or above and
    // stops there, so the Background item is still queued. Measured values -
    // Send 9, Normal 8, Render 4, Input -1, Background -2 - which is also why
    // this is the order rather than the posting order.
    [AvaloniaFact]
    public void Draining_Down_To_Input_Runs_The_Three_Above_It_In_Priority_Order()
    {
        var subject = new Ex069_DispatcherPriority();

        subject.PostAll();
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Input);

        Assert.Equal(["send", "normal", "render"], subject.Log);
    }

    [AvaloniaFact]
    public void Draining_Completely_Then_Runs_The_Background_Item_Last()
    {
        var subject = new Ex069_DispatcherPriority();

        subject.PostAll();
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Input);
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(["send", "normal", "render", "background"], subject.Log);
    }
}
