using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex069_ConverterCultureTests : WpfTestContext
{
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");
    private static readonly CultureInfo UnitedStates = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo SwissGerman = CultureInfo.GetCultureInfo("de-CH");

    [WpfFact]
    public void ConverterCulture_Wins_Even_When_The_Thread_Culture_Disagrees()
    {
        var previousCulture = Thread.CurrentThread.CurrentCulture;
        var previousUICulture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            // The thread says German; ConverterCulture below says United States. If a wrong
            // implementation formatted through the ambient thread instead of ConverterCulture,
            // this would read the German format instead.
            Thread.CurrentThread.CurrentCulture = German;
            Thread.CurrentThread.CurrentUICulture = German;

            var source = new Ex069_AmountSource { Amount = 1234.5m };
            var target = new TextBlock();

            Ex069_ConverterCulture.BindWithExplicitCulture(target, source, UnitedStates);
            Layout(target);
            Pump();

            Assert.Equal((1234.5m).ToString("C", UnitedStates), target.Text);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
            Thread.CurrentThread.CurrentUICulture = previousUICulture;
        }
    }

    [WpfFact]
    public void A_Different_ConverterCulture_Produces_A_Different_Format_And_Still_Ignores_The_Thread()
    {
        // Opposite pairing from the test above - vary which culture is "foreign" to the thread,
        // and vary the expected output too, so a mutant that happens to special-case one
        // direction cannot pass both.
        var previousCulture = Thread.CurrentThread.CurrentCulture;
        var previousUICulture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = UnitedStates;
            Thread.CurrentThread.CurrentUICulture = UnitedStates;

            var source = new Ex069_AmountSource { Amount = 1234.5m };
            var target = new TextBlock();

            Ex069_ConverterCulture.BindWithExplicitCulture(target, source, German);
            Layout(target);
            Pump();

            Assert.Equal((1234.5m).ToString("C", German), target.Text);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
            Thread.CurrentThread.CurrentUICulture = previousUICulture;
        }
    }

    [WpfFact]
    public void The_Binding_Is_Declared_With_The_Given_ConverterCulture()
    {
        var source = new Ex069_AmountSource();
        var target = new TextBlock();

        Ex069_ConverterCulture.BindWithExplicitCulture(target, source, German);

        var binding = BindingOperations.GetBinding(target, TextBlock.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(Ex069_AmountSource.Amount), binding!.Path.Path);
        Assert.Equal("{0:C}", binding.StringFormat);
        Assert.Equal(German, binding.ConverterCulture);
    }

    [WpfFact]
    public void A_Later_Amount_Change_Still_Reaches_The_Target_In_The_Same_Culture()
    {
        var source = new Ex069_AmountSource { Amount = 1234.5m };
        var target = new TextBlock();
        Ex069_ConverterCulture.BindWithExplicitCulture(target, source, German);
        Layout(target);
        Pump();

        source.Amount = 7m;
        Pump();

        Assert.Equal((7m).ToString("C", German), target.Text);
    }

    [WpfFact]
    public void Contrast_A_Binding_With_No_ConverterCulture_Ignores_The_Thread_Culture_Too()
    {
        var previousCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = SwissGerman;

            var source = new Ex069_AmountSource { Amount = 1234.5m };

            // Exercise the graded method too - the same fact as the tests above, restated with a
            // third culture, so this test fails on the stub's own NotImplementedException like
            // every other test in this row rather than only on framework behavior this row is not
            // grading.
            var explicitTarget = new TextBlock();
            Ex069_ConverterCulture.BindWithExplicitCulture(explicitTarget, source, SwissGerman);
            Layout(explicitTarget);
            Pump();
            Assert.Equal((1234.5m).ToString("C", SwissGerman), explicitTarget.Text);

            // Not this row's own API - a plain Binding built directly here, the same shape row
            // 014 uses, to show the OTHER half of the contrast this row owns: forcing the thread
            // to de-CH does not make an un-cultured binding render Swiss francs - it still falls
            // back to the target element's Language, which defaults to en-US, exactly as
            // measured in README.md's "Bindings and culture" section.
            var plainTarget = new TextBlock();
            plainTarget.DataContext = source;
            plainTarget.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex069_AmountSource.Amount)) { StringFormat = "{0:C}" });
            Layout(plainTarget);
            Pump();

            Assert.Equal("$1,234.50", plainTarget.Text);
            Assert.NotEqual((1234.5m).ToString("C", SwissGerman), plainTarget.Text);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
        }
    }
}
