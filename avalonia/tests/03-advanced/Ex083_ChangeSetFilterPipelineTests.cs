using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Advanced;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex083_ChangeSetFilterPipelineTests
{
    // Source is seeded BEFORE Start, so the very first change set already
    // describes four items rather than arriving empty - which is the measured
    // behaviour of ToReactiveChangeSet and the case a solution that only listens
    // for later changes gets wrong.
    private static Ex083_ChangeSetFilterPipeline Started(params int[] seed)
    {
        var pipeline = new Ex083_ChangeSetFilterPipeline();

        foreach (var value in seed)
        {
            pipeline.Source.Add(value);
        }

        pipeline.Start();
        return pipeline;
    }

    [AvaloniaFact]
    public void The_First_Change_Set_Already_Describes_What_Was_There()
    {
        using var pipeline = Started(1, 2, 3, 4);

        Assert.Equal([2, 4], pipeline.Filtered);
    }

    [AvaloniaFact]
    public void An_Added_Match_Appears_And_A_Non_Match_Does_Not()
    {
        using var pipeline = Started(2);

        pipeline.Source.Add(3);
        Assert.Equal([2], pipeline.Filtered);

        pipeline.Source.Add(6);
        Assert.Equal([2, 6], pipeline.Filtered);
    }

    // Order is what makes this a projection rather than a set: 4 goes in front of
    // 6 because that is where it sits in Source, not at the end because that is
    // when it arrived.
    [AvaloniaFact]
    public void A_Match_Inserted_In_The_Middle_Lands_In_The_Right_Place()
    {
        using var pipeline = Started(2, 6);

        pipeline.Source.Insert(1, 4);

        Assert.Equal([2, 4, 6], pipeline.Filtered);
    }

    [AvaloniaFact]
    public void A_Removed_Match_Disappears()
    {
        using var pipeline = Started(2, 3, 4);

        pipeline.Source.Remove(2);

        Assert.Equal([4], pipeline.Filtered);
    }

    [AvaloniaFact]
    public void Removing_A_Non_Match_Changes_Nothing()
    {
        using var pipeline = Started(2, 3, 4);

        pipeline.Source.Remove(3);

        Assert.Equal([2, 4], pipeline.Filtered);
    }

    // Replace is where a handler that only knows Add and Remove breaks, and it
    // breaks in both directions: a match replaced by a non-match has to leave, and
    // a non-match replaced by a match has to arrive - in the right position.
    [AvaloniaFact]
    public void Replacing_A_Match_With_A_Non_Match_Removes_It()
    {
        using var pipeline = Started(2, 4);

        pipeline.Source[0] = 5;

        Assert.Equal([4], pipeline.Filtered);
    }

    [AvaloniaFact]
    public void Replacing_A_Non_Match_With_A_Match_Inserts_It_In_Place()
    {
        using var pipeline = Started(2, 5, 8);

        pipeline.Source[1] = 6;

        Assert.Equal([2, 6, 8], pipeline.Filtered);
    }

    [AvaloniaFact]
    public void Replacing_One_Match_With_Another_Keeps_The_Position()
    {
        using var pipeline = Started(2, 4, 6);

        pipeline.Source[1] = 10;

        Assert.Equal([2, 10, 6], pipeline.Filtered);
    }

    [AvaloniaFact]
    public void Moving_A_Match_Moves_It_In_The_Projection()
    {
        using var pipeline = Started(2, 3, 4);

        pipeline.Source.Move(0, 2);

        Assert.Equal([4, 2], pipeline.Filtered);
    }

    // Measured: Clear() arrives as one Remove per item rather than a single reset,
    // so a handler that understands Remove empties the projection without any
    // special case for it.
    [AvaloniaFact]
    public void Clearing_The_Source_Empties_The_Projection()
    {
        using var pipeline = Started(2, 3, 4);

        pipeline.Source.Clear();

        Assert.Empty(pipeline.Filtered);
    }

    // The instance has to survive, because a view binds to it once and would not
    // follow a replacement.
    [AvaloniaFact]
    public void The_Projection_Is_Always_The_Same_Collection_Instance()
    {
        using var pipeline = Started(2);
        var instance = pipeline.Filtered;

        pipeline.Source.Add(4);
        pipeline.Source.Clear();
        pipeline.Source.Add(8);

        Assert.Same(instance, pipeline.Filtered);
        Assert.Equal([8], pipeline.Filtered);
    }

    [AvaloniaFact]
    public void Disposing_Stops_Tracking()
    {
        var pipeline = Started(2);

        pipeline.Dispose();
        pipeline.Source.Add(4);

        Assert.Equal([2], pipeline.Filtered);
    }
}
