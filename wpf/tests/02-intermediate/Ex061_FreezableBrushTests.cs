using System.Windows.Media;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex061_FreezableBrushTests : WpfTestContext
{
    // Runs action on a brand-new background thread and reports whatever it threw (or null, if
    // nothing did) - test-local plumbing, not something the exercise itself needs to ship, the
    // same "abstract plumbing only in the content library, probes test-local" shape rows
    // 040/046/057 already follow.
    private static Task<Exception?> RunOnBackgroundThreadAsync(Action action)
    {
        var tcs = new TaskCompletionSource<Exception?>();
        var thread = new Thread(() =>
        {
            try
            {
                action();
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetResult(ex);
            }
        })
        {
            IsBackground = true,
        };
        thread.Start();
        return tcs.Task;
    }

    [WpfFact]
    public void FreezeIfPossible_Freezes_A_Plain_Brush_And_Returns_True()
    {
        var brush = new SolidColorBrush(Colors.Red);

        var result = Ex061_FreezableBrush.FreezeIfPossible(brush);

        // Load-bearing against "freeze a clone and return the original": the ORIGINAL instance
        // passed in must itself end up frozen, not merely the return value claiming so.
        Assert.True(result);
        Assert.True(brush.IsFrozen);
    }

    [WpfFact]
    public void FreezeIfPossible_Leaves_An_Animated_Brush_Unfrozen_And_Returns_False_Without_Throwing()
    {
        var brush = new SolidColorBrush(Colors.Blue);
        // An active animation makes CanFreeze false - Freeze() on this brush would throw if
        // called unconditionally.
        brush.BeginAnimation(SolidColorBrush.ColorProperty, new System.Windows.Media.Animation.ColorAnimation(Colors.Green, TimeSpan.FromSeconds(10)));

        var result = Ex061_FreezableBrush.FreezeIfPossible(brush);

        Assert.False(result);
        Assert.False(brush.IsFrozen);
    }

    [WpfFact]
    public void FreezeIfPossible_Is_Idempotent_On_An_Already_Frozen_Brush()
    {
        var brush = new SolidColorBrush(Colors.Black);

        var first = Ex061_FreezableBrush.FreezeIfPossible(brush);
        // Against a mutant that assumes it is only ever called once and mishandles an
        // already-frozen instance (throwing, or trying to "unfreeze" it): calling it again must
        // be a harmless no-op.
        var second = Ex061_FreezableBrush.FreezeIfPossible(brush);

        Assert.True(first);
        Assert.True(second);
        Assert.True(brush.IsFrozen);
    }

    [WpfFact]
    public void CreateFrozenBrush_Returns_A_Frozen_Brush_With_The_Given_Color()
    {
        var brush = Ex061_FreezableBrush.CreateFrozenBrush(Colors.Lime);

        Assert.True(brush.IsFrozen);
        Assert.Equal(Colors.Lime, brush.Color);
    }

    [WpfFact]
    public async Task An_Unfrozen_Freezable_Throws_When_Touched_From_A_Different_Thread()
    {
        var brush = new SolidColorBrush(Colors.Purple);
        brush.BeginAnimation(SolidColorBrush.ColorProperty, new System.Windows.Media.Animation.ColorAnimation(Colors.Cyan, TimeSpan.FromSeconds(10)));

        // Goes through FreezeIfPossible first (an animated brush can never freeze) so this test
        // exercises the stub too, rather than only probing raw platform behavior against a brush
        // the exercise never touched.
        var result = Ex061_FreezableBrush.FreezeIfPossible(brush);
        Assert.False(result);

        var thrown = await WithTimeout(RunOnBackgroundThreadAsync(() => _ = brush.Color));

        Assert.IsType<InvalidOperationException>(thrown);
    }

    [WpfFact]
    public async Task Freezing_Removes_Thread_Affinity_So_Another_Thread_Can_Read_It()
    {
        var brush = Ex061_FreezableBrush.CreateFrozenBrush(Colors.Orange);
        Assert.True(brush.IsFrozen);

        var thrown = await WithTimeout(RunOnBackgroundThreadAsync(() => _ = brush.Color));

        Assert.Null(thrown);
    }
}
