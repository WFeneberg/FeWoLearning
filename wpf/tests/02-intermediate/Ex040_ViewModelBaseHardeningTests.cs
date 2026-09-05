using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex040_ViewModelBaseHardeningTests : WpfTestContext
{
    private sealed class ComparerProbe(IEqualityComparer<string>? comparer) : Ex040_ObservableViewModelBase
    {
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, comparer);
        }
    }

    private sealed class ReentrancyProbe : Ex040_ObservableViewModelBase
    {
        private int _value;
        private string _label = string.Empty;

        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }
    }

    private sealed class FanOutProbe : Ex040_ObservableViewModelBase
    {
        private double _celsius;

        public double Celsius
        {
            get => _celsius;
            set
            {
                if (!SetProperty(ref _celsius, value)) return;
                RaisePropertyChanged(nameof(Fahrenheit));
                RaisePropertyChanged(nameof(IsFreezing));
            }
        }

        public double Fahrenheit => _celsius * 9.0 / 5.0 + 32.0;

        public bool IsFreezing => _celsius <= 0.0;
    }

    private static List<string?> Record(Ex040_ObservableViewModelBase model)
    {
        var names = new List<string?>();
        model.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    [WpfFact]
    public void A_Custom_Comparer_Treats_Case_Insensitive_Equal_Strings_As_No_Change()
    {
        var probe = new ComparerProbe(StringComparer.OrdinalIgnoreCase) { Name = "Wolfgang" };
        var names = Record(probe);

        probe.Name = "WOLFGANG";

        // A SetProperty that ignores the supplied comparer (always falling back to the
        // default) would see these as different and both raise and overwrite.
        Assert.Empty(names);
        Assert.Equal("Wolfgang", probe.Name);
    }

    [WpfFact]
    public void Without_An_Explicit_Comparer_The_Default_Still_Distinguishes_Case()
    {
        var probe = new ComparerProbe(comparer: null) { Name = "Wolfgang" };
        var names = Record(probe);

        probe.Name = "WOLFGANG";

        Assert.Equal(new string?[] { nameof(ComparerProbe.Name) }, names);
        Assert.Equal("WOLFGANG", probe.Name);
    }

    [WpfFact]
    public void SetProperty_Still_Supports_The_Familiar_Fan_Out_From_Row_010()
    {
        var probe = new FanOutProbe();
        var names = Record(probe);

        probe.Celsius = 100;

        Assert.Equal(new string?[] { "Celsius", "Fahrenheit", "IsFreezing" }, names);
    }

    [WpfFact]
    public void SetProperty_Raises_Nothing_On_An_Equal_Value_Even_With_Fan_Out_Wired_Up()
    {
        var probe = new FanOutProbe { Celsius = 20 };
        var names = Record(probe);

        probe.Celsius = 20;

        Assert.Empty(names);
    }

    [WpfFact]
    public void A_Handler_Setting_The_Same_Property_Again_Does_Not_Cause_A_Second_Nested_Raise()
    {
        var probe = new ReentrancyProbe();
        var raiseCount = 0;
        var reentered = false;

        probe.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(ReentrancyProbe.Value)) return;
            raiseCount++;
            if (!reentered)
            {
                reentered = true;
                probe.Value = 2; // reentrant set of the SAME property from inside its own handler
            }
        };

        probe.Value = 1;

        // Without the guard, the reentrant SetProperty call raises PropertyChanged again
        // while the outer raise's single handler is still on the stack, so that same
        // handler runs a second time and this count reaches 2.
        Assert.Equal(1, raiseCount);
        Assert.Equal(2, probe.Value); // the reentrant write still took effect
    }

    [WpfFact]
    public void The_Guard_Is_Specific_To_The_Property_Being_Raised_Not_Global()
    {
        var probe = new ReentrancyProbe();
        var labelRaises = 0;

        probe.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ReentrancyProbe.Value))
            {
                probe.Label = "changed-from-value-handler"; // a DIFFERENT property - must still raise
            }

            if (e.PropertyName == nameof(ReentrancyProbe.Label))
            {
                labelRaises++;
            }
        };

        probe.Value = 5;

        // A guard implemented as one global "am I raising anything at all" flag - instead
        // of one scoped per property name - would wrongly suppress this too.
        Assert.Equal(1, labelRaises);
        Assert.Equal("changed-from-value-handler", probe.Label);
    }
}
