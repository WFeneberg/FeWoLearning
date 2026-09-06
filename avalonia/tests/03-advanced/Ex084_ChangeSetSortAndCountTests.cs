using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Advanced;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex084_ChangeSetSortAndCountTests
{
    private static Ex084_ChangeSetSortAndCount Started(params int[] seed)
    {
        var subject = new Ex084_ChangeSetSortAndCount();

        foreach (var value in seed)
        {
            subject.Source.Add(value);
        }

        subject.Start();
        return subject;
    }

    [AvaloniaFact]
    public void The_Seeded_Values_Are_Sorted_Immediately()
    {
        using var subject = Started(5, 1, 9, 3);

        Assert.Equal([1, 3, 5, 9], subject.Sorted);
    }

    [AvaloniaFact]
    public void An_Added_Value_Lands_In_Its_Sorted_Place()
    {
        using var subject = Started(1, 9);

        subject.Source.Add(4);
        Assert.Equal([1, 4, 9], subject.Sorted);

        subject.Source.Add(0);
        Assert.Equal([0, 1, 4, 9], subject.Sorted);

        subject.Source.Add(12);
        Assert.Equal([0, 1, 4, 9, 12], subject.Sorted);
    }

    [AvaloniaFact]
    public void A_Removed_Value_Leaves_The_Order_Intact()
    {
        using var subject = Started(5, 1, 9);

        subject.Source.Remove(5);

        Assert.Equal([1, 9], subject.Sorted);
    }

    [AvaloniaFact]
    public void A_Replaced_Value_Is_Re_Sorted_Rather_Than_Swapped_In_Place()
    {
        using var subject = Started(1, 5, 9);

        subject.Source[1] = 20;

        Assert.Equal([1, 9, 20], subject.Sorted);
    }

    // The difference from ex083's filtered projection, and the reason both rows
    // exist: sorted order does not depend on the source order, so a Move must
    // change nothing. An implementation that reuses ex083's Move handling by
    // reflex reorders the sorted list and fails here.
    [AvaloniaFact]
    public void Moving_A_Value_In_The_Source_Leaves_The_Sorted_Order_Alone()
    {
        using var subject = Started(1, 5, 9);

        subject.Source.Move(0, 2);

        Assert.Equal([1, 5, 9], subject.Sorted);
        Assert.Equal([5, 9, 1], subject.Source);
    }

    [AvaloniaFact]
    public void The_Sorted_Projection_Is_Always_The_Same_Instance()
    {
        using var subject = Started(3);
        var instance = subject.Sorted;

        subject.Source.Add(1);
        subject.Source.Clear();
        subject.Source.Add(7);

        Assert.Same(instance, subject.Sorted);
        Assert.Equal([7], subject.Sorted);
    }

    // Subscribing reports the size the collection already has - measured, the
    // first change set describes the existing items - so a consumer sees a value
    // without having to wait for the first mutation.
    [AvaloniaFact]
    public void Subscribing_Reports_The_Size_Already_There()
    {
        using var subject = Started(5, 1);

        Assert.Equal([2], subject.ReportedCounts);
    }

    [AvaloniaFact]
    public void Adds_And_Removes_Each_Report_The_New_Size()
    {
        using var subject = Started(5);

        subject.Source.Add(1);
        subject.Source.Add(2);
        subject.Source.Remove(5);

        Assert.Equal([1, 2, 3, 2], subject.ReportedCounts);
    }

    // The subtle half. A Replace changes the contents but not the size, so it must
    // not be reported at all: an implementation that appends Source.Count on every
    // change set passes every test above and fails this one.
    [AvaloniaFact]
    public void A_Replace_Reports_Nothing_Because_The_Size_Did_Not_Change()
    {
        using var subject = Started(5, 1);

        subject.Source[0] = 50;
        subject.Source[1] = 10;

        Assert.Equal([2], subject.ReportedCounts);
        Assert.Equal([10, 50], subject.Sorted);
    }

    // A Move is the same story from the other side, and it is worth its own check
    // because Move and Replace reach the count operator by different routes.
    [AvaloniaFact]
    public void A_Move_Reports_Nothing_Either()
    {
        using var subject = Started(5, 1, 9);

        subject.Source.Move(2, 0);

        Assert.Equal([3], subject.ReportedCounts);
    }

    [AvaloniaFact]
    public void Disposing_Stops_Both_Projections()
    {
        var subject = Started(5);

        subject.Dispose();
        subject.Source.Add(1);

        Assert.Equal([5], subject.Sorted);
        Assert.Equal([1], subject.ReportedCounts);
    }
}
