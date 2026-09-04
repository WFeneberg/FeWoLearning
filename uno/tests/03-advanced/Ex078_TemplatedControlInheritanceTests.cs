using FeWoLearning.Uno.Exercises.Advanced;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex078_TemplatedControlInheritanceTests : UnoTestContext
{
    private static Ex078_TemplatedControlInheritance Badge(bool available = true, bool urgent = false) =>
        Layout(new Ex078_TemplatedControlInheritance
        {
            Template = Ex078_BadgeBase.SharedTemplate,
            IsAvailable = available,
            IsUrgent = urgent,
        });

    [Fact]
    public void The_Subclass_Claims_Its_Own_Default_Style()
    {
        var badge = Badge();

        // Leaving the base's key in place makes the subclass look up the base's default
        // style, so its own is never found - and nothing reports it.
        Assert.Equal(typeof(Ex078_TemplatedControlInheritance), badge.DeclaredStyleKey);
    }

    [Fact]
    public void The_Base_Still_Finds_Its_Part()
    {
        var badge = Badge();

        // base.OnApplyTemplate is what binds this. A subclass that forgets it leaves the
        // base's parts null while everything still compiles.
        Assert.NotNull(badge.Fill);
    }

    [Fact]
    public void The_Base_State_Still_Applies()
    {
        var badge = Badge(available: false);

        Assert.Equal(0.4, badge.Fill!.Opacity, 2);
    }

    [Fact]
    public void The_Subclass_State_Applies()
    {
        var badge = Badge(urgent: true);

        Assert.Equal(60, badge.Fill!.Width, 1);
    }

    [Fact]
    public void Both_States_Apply_Together()
    {
        var badge = Badge(available: false, urgent: true);

        Assert.Equal(0.4, badge.Fill!.Opacity, 2);
        Assert.Equal(60, badge.Fill.Width, 1);
    }

    [Fact]
    public void A_Base_Property_Change_Keeps_The_Subclass_State()
    {
        var badge = Badge(urgent: true);

        badge.IsAvailable = false;
        badge.IsAvailable = true;

        // The base raises the update, the override runs, and both groups are re-entered.
        // An override that does not call the base loses availability; one the base does not
        // route through loses urgency.
        Assert.Equal(60, badge.Fill!.Width, 1);
    }

    [Fact]
    public void A_Subclass_Property_Change_Keeps_The_Base_State()
    {
        var badge = Badge(available: false);

        badge.IsUrgent = true;

        Assert.Equal(0.4, badge.Fill!.Opacity, 2);
    }

    [Fact]
    public void The_Subclass_Update_Runs_For_Base_Changes_Too()
    {
        var badge = Badge();
        var before = badge.StateUpdates;

        badge.IsAvailable = false;

        // The base calls the virtual, so the subclass gets to add its states on every
        // update - not only on its own property changes.
        Assert.Equal(before + 1, badge.StateUpdates);
    }

    [Fact]
    public void A_Late_Template_Comes_Up_In_Both_States()
    {
        var badge = new Ex078_TemplatedControlInheritance { IsAvailable = false, IsUrgent = true };

        badge.Template = Ex078_BadgeBase.SharedTemplate;
        Layout(badge);

        Assert.Equal(0.4, badge.Fill!.Opacity, 2);
        Assert.Equal(60, badge.Fill.Width, 1);
    }
}
