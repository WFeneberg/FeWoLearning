using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex043_ThrottledSearchTests
{
    [Fact]
    public void Initial_CommittedQuery_Is_Empty_And_Nothing_Has_Searched_Yet()
    {
        var vm = new Ex043_ThrottledSearchViewModel(new VirtualClock());

        Assert.Equal(string.Empty, vm.CommittedQuery);
        Assert.Equal(0, vm.SearchCount);
    }

    // The Throttle discriminator: a solution with no Throttle at all would commit
    // every keystroke immediately. Driving three rapid writes and checking nothing
    // commits until the full 300ms window has elapsed - not 299ms, not immediately -
    // is what a naive pass-through implementation cannot satisfy.
    [Fact]
    public void Rapid_Keystrokes_Are_Absorbed_Until_The_Full_Window_Elapses()
    {
        var vt = new VirtualClock();
        var vm = new Ex043_ThrottledSearchViewModel(vt);
        var seen = new List<string>();
        using var sub = vm.WhenAnyValue(x => x.CommittedQuery).Subscribe(v => seen.Add(v));

        Assert.Equal(new[] { "" }, seen);

        vm.Query = "a";
        vm.Query = "ab";
        vm.Query = "abc";
        Assert.Equal(new[] { "" }, seen);

        vt.AdvanceBy(TimeSpan.FromMilliseconds(299));
        Assert.Equal(new[] { "" }, seen);

        vt.AdvanceBy(TimeSpan.FromMilliseconds(2));
        Assert.Equal(new[] { "", "abc" }, seen);
    }

    // The DistinctUntilChanged discriminator. "abc" is committed once above; typing
    // through a detour ("xyz") and settling back on "abc" produces a genuine second
    // Query change (ReactiveObject only suppresses an assignment that equals the
    // CURRENT value, and "abc" is not the current value at that point - it is "xyz"),
    // and "xyz" itself never survives its own throttle window (superseded by "abc"
    // before it settles), so Throttle alone would still commit "abc" a SECOND time,
    // consecutively, as its own distinct emission.
    //
    // CommittedQuery's own PropertyChanged cannot see that second emission even
    // without DistinctUntilChanged - ObservableAsPropertyHelper suppresses a
    // consecutive re-assignment of the same value on its own, exactly like a plain
    // property would. SearchCount is the discriminator that actually depends on
    // DistinctUntilChanged: it is a second, independent subscriber to the same
    // Throttle output with no such built-in suppression, so it only stays at 1
    // through the "xyz -> abc" detour if DistinctUntilChanged is genuinely filtering
    // the pipeline upstream of both subscribers.
    //
    // KNOWN, DELIBERATE GAP: this assertion is only sound if SearchCount is wired the
    // way the stub's TODO instructs - as a second Subscribe on the PRE-ToProperty
    // observable (WhenAnyValue.Throttle.DistinctUntilChanged), not on CommittedQuery
    // itself. A learner who instead increments SearchCount from
    // this.WhenAnyValue(x => x.CommittedQuery) on a Throttle-only pipeline (no
    // DistinctUntilChanged at all) still passes all three tests in this file: the
    // OAPH backing CommittedQuery de-duplicates the consecutive "abc" for them, for
    // free, so a counter downstream of CommittedQuery never sees the duplicate
    // either - the exact hole SearchCount exists to close, one level removed. There
    // is no clean structural assertion that closes this without also rejecting
    // legitimate variants (this track has twice shipped an over-constraining
    // assertion of that kind), and the alternate wiring is unusual enough, and
    // already called out by name in the stub ("not to CommittedQuery itself"), that
    // this is enforced by instruction rather than by a test. This is a deliberate
    // choice, not an oversight.
    [Fact]
    public void Re_Settling_On_The_Same_Value_Does_Not_Search_Again()
    {
        var vt = new VirtualClock();
        var vm = new Ex043_ThrottledSearchViewModel(vt);

        vm.Query = "abc";
        vt.AdvanceBy(TimeSpan.FromMilliseconds(301));
        Assert.Equal("abc", vm.CommittedQuery);
        Assert.Equal(1, vm.SearchCount);

        vm.Query = "xyz";
        vm.Query = "abc";
        vt.AdvanceBy(TimeSpan.FromMilliseconds(301));
        Assert.Equal("abc", vm.CommittedQuery);
        Assert.Equal(1, vm.SearchCount);

        vm.Query = "abcd";
        vt.AdvanceBy(TimeSpan.FromMilliseconds(301));
        Assert.Equal("abcd", vm.CommittedQuery);
        Assert.Equal(2, vm.SearchCount);
    }
}
