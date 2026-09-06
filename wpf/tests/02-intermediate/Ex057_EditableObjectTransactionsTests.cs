using System.ComponentModel;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex057_EditableObjectTransactionsTests : WpfTestContext
{
    [WpfFact]
    public void BeginEdit_Then_CancelEdit_Restores_The_Original_Values()
    {
        var person = new Ex057_EditablePerson { Name = "Ada", Age = 30 };

        person.BeginEdit();
        person.Name = "Grace";
        person.Age = 99;
        person.CancelEdit();

        // Against a BeginEdit that snapshots nothing: there would be no original state to
        // restore to, and these would still read "Grace"/99.
        Assert.Equal("Ada", person.Name);
        Assert.Equal(30, person.Age);
    }

    [WpfFact]
    public void A_Different_Person_And_Property_Also_Restores_On_Cancel()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance - a different
        // starting state and only Age changed this time, not Name.
        var person = new Ex057_EditablePerson { Name = "Bob", Age = 40 };

        person.BeginEdit();
        person.Age = 41;
        person.CancelEdit();

        Assert.Equal("Bob", person.Name);
        Assert.Equal(40, person.Age);
    }

    [WpfFact]
    public void EndEdit_Commits_The_New_Values_Instead_Of_Reverting()
    {
        var person = new Ex057_EditablePerson { Name = "Carol", Age = 50 };

        person.BeginEdit();
        person.Name = "Caroline";
        person.EndEdit();

        // Against an EndEdit that discards instead of committing (behaves like CancelEdit):
        // that would leave "Carol" here instead.
        Assert.Equal("Caroline", person.Name);
        Assert.Equal(50, person.Age);
    }

    [WpfFact]
    public void A_Second_BeginEdit_Before_EndEdit_Does_Not_Overwrite_The_Original_Snapshot()
    {
        var person = new Ex057_EditablePerson { Name = "Orig", Age = 1 };

        person.BeginEdit();
        person.Name = "A";
        person.BeginEdit(); // nested/re-entrant call while already mid-edit - must be a no-op
        person.Name = "B";
        person.CancelEdit();

        // Against a second BeginEdit that re-snapshots (overwriting the first snapshot with the
        // mid-edit state "A"): CancelEdit would then only roll back to "A", not all the way to
        // the state before the FIRST BeginEdit.
        Assert.Equal("Orig", person.Name);
        Assert.Equal(1, person.Age);
    }

    [WpfFact]
    public void CancelEdit_Raises_PropertyChanged_For_Whatever_It_Restores()
    {
        // Against a CancelEdit that restores by swapping backing fields directly instead of
        // going through the property setters: a real bound TextBox would never learn the value
        // changed back. This proves the restore goes through the property, not around it.
        var person = new Ex057_EditablePerson { Name = "Dana", Age = 20 };
        var raised = new List<string>();
        person.PropertyChanged += (_, e) => raised.Add(e.PropertyName ?? "");

        person.BeginEdit();
        person.Name = "Temp";
        person.Age = 99;
        raised.Clear();

        person.CancelEdit();

        // Against a mutant restoring ONE property through its setter and the OTHER by poking the
        // backing field directly (a partial bypass a Name-only assertion could not catch): both
        // properties' restores must go through their setters, so both must raise PropertyChanged.
        Assert.Contains(nameof(Ex057_EditablePerson.Name), raised);
        Assert.Contains(nameof(Ex057_EditablePerson.Age), raised);
        Assert.Equal("Dana", person.Name);
        Assert.Equal(20, person.Age);
    }

    [WpfFact]
    public void EndEdit_Clears_The_Snapshot_So_A_Later_CancelEdit_Is_A_NoOp()
    {
        var person = new Ex057_EditablePerson { Name = "Ada" };

        person.BeginEdit();
        person.Name = "Grace";
        person.EndEdit();
        person.CancelEdit(); // nothing is being edited anymore - must do nothing

        // Against an EndEdit that commits the values but leaves the snapshot in place (an empty
        // body, say): this later, unrelated CancelEdit would still find a snapshot to roll back
        // to and incorrectly revert to "Ada".
        Assert.Equal("Grace", person.Name);
    }

    [WpfFact]
    public void CancelEdit_Clears_The_Snapshot_So_A_Later_Edit_With_No_BeginEdit_Is_Unaffected()
    {
        var person = new Ex057_EditablePerson { Name = "Ada" };

        person.BeginEdit();
        person.Name = "Grace";
        person.CancelEdit();
        person.Name = "Hopper"; // a plain edit outside any transaction - no BeginEdit here
        person.CancelEdit(); // nothing is being edited - must do nothing

        // Against a CancelEdit that restores correctly but never clears the snapshot: the stale
        // snapshot from the FIRST BeginEdit would still be sitting there, and this second,
        // unrelated CancelEdit call would incorrectly roll back to "Ada" again.
        Assert.Equal("Hopper", person.Name);
    }
}
