using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex044_DataErrorInfoLegacyTests : WpfTestContext
{
    // A concrete validating view model, deliberately test-local - not shipped in the content
    // library, for the same reason row 043's probes are test-local.
    private sealed class NameProbe : Ex044_LegacyValidatingViewModelBase
    {
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        protected override string? GetError(string propertyName)
            => propertyName == nameof(Name) && string.IsNullOrWhiteSpace(_name) ? "Required" : null;
    }

    [WpfTheory]
    [InlineData("")]
    [InlineData("   ")]
    public void The_Indexer_Reports_An_Error_Through_The_Interface_For_An_Invalid_Value(string invalidName)
    {
        var probe = new NameProbe { Name = invalidName };

        // Only reachable through the interface reference - IDataErrorInfo.this[] is an
        // explicit interface implementation, so a side-channel property could not be
        // substituted here even by accident.
        IDataErrorInfo dataErrorInfo = probe;

        Assert.Equal("Required", dataErrorInfo["Name"]);
    }

    [WpfFact]
    public void The_Indexer_Reports_No_Error_Through_The_Interface_For_A_Valid_Value()
    {
        var probe = new NameProbe { Name = "Wolfgang" };
        IDataErrorInfo dataErrorInfo = probe;

        Assert.Equal(string.Empty, dataErrorInfo["Name"]);
    }

    [WpfFact]
    public void BindWithLegacyValidation_Declares_TwoWay_PropertyChanged_And_Explicitly_Enables_The_Legacy_Flag()
    {
        var source = new NameProbe();
        var target = new TextBox();

        Ex044_DataErrorInfoLegacy.BindWithLegacyValidation(target, source, nameof(NameProbe.Name));

        var binding = BindingOperations.GetBinding(target, TextBox.TextProperty);
        Assert.NotNull(binding);
        Assert.Equal(nameof(NameProbe.Name), binding!.Path.Path);
        Assert.Equal(BindingMode.TwoWay, binding.Mode);
        Assert.Equal(UpdateSourceTrigger.PropertyChanged, binding.UpdateSourceTrigger);

        // Load-bearing: this is the row's own named flag. Measured to default false - a
        // BindWithLegacyValidation that wires everything else correctly but forgets this
        // line leaves the whole mechanism silently inert.
        Assert.True(binding.ValidatesOnDataErrors);
    }

    [WpfFact]
    public void A_Bound_TextBox_Surfaces_The_Legacy_Validation_Error_Once_The_Flag_Is_Set()
    {
        var source = new NameProbe { Name = "Wolfgang" };
        var target = new TextBox();

        Ex044_DataErrorInfoLegacy.BindWithLegacyValidation(target, source, nameof(NameProbe.Name));
        Layout(target);
        Pump();

        target.Text = "";
        Pump();

        Assert.True(Validation.GetHasError(target));
        var errors = Validation.GetErrors(target);
        Assert.Single(errors);
        Assert.Equal("Required", errors[0].ErrorContent);

        target.Text = "Feneberg";
        Pump();

        Assert.False(Validation.GetHasError(target));
    }
}
