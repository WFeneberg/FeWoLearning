using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex014_StringFormatAndFallbacksTests : WpfTestContext
{
    // Pins the display culture explicitly so "{0:C}" is deterministic on every machine
    // this suite runs on: a Binding takes its format culture from
    // Binding.ConverterCulture, falling back to the target element's Language property
    // (default en-US) - never from Thread.CurrentCulture/the OS locale. This machine
    // is de-CH with UI culture de-DE; without this, "{0:C}" would render whatever
    // currency the OS happens to be set to, which is exactly the kind of test that
    // passes here and fails elsewhere. Pinning Language, not ConverterCulture, keeps
    // this row about StringFormat/FallbackValue/TargetNullValue - ConverterCulture
    // itself is row 069's subject.
    private static TextBlock NewTarget() => new() { Language = XmlLanguage.GetLanguage("en-US") };

    [WpfFact]
    public void A_Present_Amount_Is_Formatted_As_Currency()
    {
        var source = new Ex014_InvoiceSource { Amount = 1234.5m };
        var target = NewTarget();

        Ex014_StringFormatAndFallbacks.Bind(target, source);
        Layout(target);
        Pump();

        Assert.Equal("$1,234.50", target.Text);
    }

    [WpfFact]
    public void A_Later_Amount_Change_Still_Reaches_The_Target()
    {
        var source = new Ex014_InvoiceSource { Amount = 1234.5m };
        var target = NewTarget();
        Ex014_StringFormatAndFallbacks.Bind(target, source);
        Layout(target);
        Pump();

        source.Amount = 7m;
        Pump();

        // Rules out a hard-coded literal satisfying the first test: this is a live
        // binding, not a one-time copy.
        Assert.Equal("$7.00", target.Text);
    }

    [WpfFact]
    public void A_Null_Amount_On_A_Real_Invoice_Shows_The_TargetNullValue()
    {
        var source = new Ex014_InvoiceSource { Amount = null };
        var target = NewTarget();

        Ex014_StringFormatAndFallbacks.Bind(target, source);
        Layout(target);
        Pump();

        Assert.Equal("no amount yet", target.Text);
    }

    [WpfFact]
    public void No_Invoice_At_All_Shows_The_FallbackValue()
    {
        var target = NewTarget();

        Ex014_StringFormatAndFallbacks.Bind(target, null);
        Layout(target);
        Pump();

        // Different failure shape from the null-Amount case above: here the binding
        // cannot resolve a value at all, because there is no source to read Amount
        // from - not the same as a source whose Amount happens to be null.
        Assert.Equal("no invoice", target.Text);
    }

    [WpfFact]
    public void The_Binding_Is_Declared_With_The_Expected_Path_Format_And_Fallbacks()
    {
        var source = new Ex014_InvoiceSource();
        var target = NewTarget();

        Ex014_StringFormatAndFallbacks.Bind(target, source);

        var binding = BindingOperations.GetBinding(target, TextBlock.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(Ex014_InvoiceSource.Amount), binding!.Path.Path);
        Assert.Equal("{0:C}", binding.StringFormat);
        Assert.Equal("no invoice", binding.FallbackValue);
        Assert.Equal("no amount yet", binding.TargetNullValue);
    }
}
