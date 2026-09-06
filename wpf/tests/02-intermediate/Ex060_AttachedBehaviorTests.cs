using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex060_AttachedBehaviorTests : WpfTestContext
{
    [WpfFact]
    public void Setting_AutoUppercase_True_Forces_New_Text_To_Uppercase()
    {
        var textBox = new TextBox();

        // Through SetValue directly, not the CLR wrapper - proves the behavior is wired from the
        // property system's own callback, not from some other code path only the convenience
        // setter happens to run (see wpf/README.md's rule on writing through SetValue).
        textBox.SetValue(Ex060_AttachedBehavior.AutoUppercaseProperty, true);
        textBox.Text = "abc";

        Assert.Equal("ABC", textBox.Text);
    }

    [WpfFact]
    public void Clearing_AutoUppercase_Detaches_The_Handler_Completely()
    {
        var textBox = new TextBox();
        Ex060_AttachedBehavior.SetAutoUppercase(textBox, true);
        textBox.Text = "abc";
        Assert.Equal("ABC", textBox.Text);

        // Against a bypass that attaches on set but never detaches on clear: this would still
        // uppercase after AutoUppercase was cleared.
        Ex060_AttachedBehavior.SetAutoUppercase(textBox, false);
        textBox.Text = "def";

        Assert.Equal("def", textBox.Text);
    }

    [WpfFact]
    public void A_TextBox_Never_Attached_Is_Never_Affected_By_A_Different_One_Being_Attached()
    {
        var attached = new TextBox();
        Ex060_AttachedBehavior.SetAutoUppercase(attached, true);
        attached.Text = "abc";
        Assert.Equal("ABC", attached.Text);

        // Against a bypass that wires the handler globally (e.g. a static constructor or
        // EventManager.RegisterClassHandler covering every TextBox) instead of from the attached
        // property's own callback: a completely separate TextBox that never had AutoUppercase
        // touched at all would still get uppercased here.
        var neverAttached = new TextBox();
        neverAttached.Text = "xyz";

        Assert.Equal("xyz", neverAttached.Text);
    }

    [WpfFact]
    public void A_Different_TextBox_And_Text_Also_Uppercases_When_Attached()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance.
        var textBox = new TextBox();
        Ex060_AttachedBehavior.SetAutoUppercase(textBox, true);

        textBox.Text = "Mixed Case 123";

        Assert.Equal("MIXED CASE 123", textBox.Text);
    }
}
