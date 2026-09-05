using FeWoLearning.Architecture.Exercises.Web.Ex013;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex013_PaginationContractTests
{
    private static DateTimeOffset At(int minute) =>
        new(2026, 1, 1, 0, minute, 0, TimeSpan.Zero);

    /// <summary>Five items, two of which deliberately share a CreatedAt.</summary>
    private static List<Item> Sample() =>
    [
        new("b", At(2)),
        new("a", At(2)),   // tie with "b" on CreatedAt
        new("c", At(3)),
        new("d", At(4)),
        new("e", At(5)),
    ];

    [Fact]
    public void Offset_Returns_The_Requested_Window()
    {
        var page = Ex013_PaginationContract.OffsetPage(Sample(), skip: 0, take: 2);

        Assert.Equal(["a", "b"], page.Items.Select(i => i.Id));
    }

    [Fact]
    public void Adversarial_Ordering_Breaks_Ties_So_The_Sequence_Is_Repeatable()
    {
        // "a" and "b" share a CreatedAt. Ordering by CreatedAt alone leaves their
        // relative order undefined - and an undefined order is allowed to differ
        // between the request that fetched page 1 and the one that fetched page 2,
        // which is precisely how a row goes missing. Reversing the input must not
        // change the output.
        var forward = Ex013_PaginationContract.OffsetPage(Sample(), 0, 5);

        var reversed = Sample();
        reversed.Reverse();
        var backward = Ex013_PaginationContract.OffsetPage(reversed, 0, 5);

        Assert.Equal(["a", "b", "c", "d", "e"], forward.Items.Select(i => i.Id));
        Assert.Equal(forward.Items.Select(i => i.Id), backward.Items.Select(i => i.Id));
    }

    [Fact]
    public void Cursor_Resumes_Strictly_After_The_Cursor_And_Reports_The_Next_One()
    {
        var source = Sample();

        var first = Ex013_PaginationContract.CursorPage(source, cursor: null, take: 2);
        Assert.Equal(["a", "b"], first.Items.Select(i => i.Id));
        Assert.Equal("b", first.NextCursor);

        var second = Ex013_PaginationContract.CursorPage(source, first.NextCursor, take: 2);
        Assert.Equal(["c", "d"], second.Items.Select(i => i.Id));
    }

    [Fact]
    public void Cursor_Is_Null_On_The_Last_Page()
    {
        var source = Sample();

        var last = Ex013_PaginationContract.CursorPage(source, cursor: "d", take: 2);

        Assert.Equal(["e"], last.Items.Select(i => i.Id));
        Assert.Null(last.NextCursor);
    }

    [Fact]
    public void Mechanism_An_Insert_Between_Pages_Makes_Offset_Repeat_A_Row()
    {
        // The whole exercise. Fetch page 1, let a row appear EARLIER in the order, then
        // fetch page 2 the way a client would - by asking for the next window.
        var source = Sample();

        var firstPage = Ex013_PaginationContract.OffsetPage(source, skip: 0, take: 2);
        Assert.Equal(["a", "b"], firstPage.Items.Select(i => i.Id));

        source.Add(new Item("aa", At(1))); // sorts before everything already seen

        var secondPage = Ex013_PaginationContract.OffsetPage(source, skip: 2, take: 2);

        // "b" is served twice, and one row further down will never be served at all.
        Assert.Contains("b", secondPage.Items.Select(i => i.Id));
    }

    [Fact]
    public void Mechanism_The_Same_Insert_Leaves_Cursor_Pagination_Correct()
    {
        // Same data, same interleaving, different contract. The cursor names a position
        // in the order rather than a count of rows skipped, so a row appearing before
        // it cannot shift the window.
        var source = Sample();

        var firstPage = Ex013_PaginationContract.CursorPage(source, cursor: null, take: 2);
        Assert.Equal(["a", "b"], firstPage.Items.Select(i => i.Id));

        source.Add(new Item("aa", At(1)));

        var secondPage = Ex013_PaginationContract.CursorPage(source, firstPage.NextCursor, take: 2);

        Assert.Equal(["c", "d"], secondPage.Items.Select(i => i.Id));
    }
}
