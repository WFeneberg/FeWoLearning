using System.Globalization;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex013_BindingStringFormatTests
{
    private static (Ex013_BindingStringFormat View, Ex013_BindingStringFormatViewModel Vm) Arrange()
    {
        var vm = new Ex013_BindingStringFormatViewModel();
        var view = ViewHarness.Show(new Ex013_BindingStringFormat { DataContext = vm }, 300, 100);
        return (view, vm);
    }

    // StringFormat renders through CultureInfo.CurrentCulture - the ambient culture
    // of whatever machine runs the test - not a culture fixed by the binding. This
    // repo's dev machine is de-CH, where {0:N2} on 1234.5 renders "1'234.50" with
    // U+2019 as the thousands separator; CI or another contributor's machine could
    // be anything else. Pin a known culture for the whole arrange/act/assert here
    // so the result is identical everywhere, rather than asserting whatever the
    // ambient culture happens to produce.
    [AvaloniaFact]
    public void Formats_With_Thousands_Grouping_Under_A_Pinned_Culture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            var (view, vm) = Arrange();
            var text = view.FindControl<TextBlock>("AmountText")!;

            Assert.Equal("1,234.50", text.Text);

            // Mutate and re-assert: a hard-coded literal string in the view would
            // render the correct initial value once and then never change.
            vm.Amount = 2500.4m;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("2,500.40", text.Text);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
