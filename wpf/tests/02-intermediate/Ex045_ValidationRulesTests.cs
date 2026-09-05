using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex045_ValidationRulesTests : WpfTestContext
{
    private sealed class NameSource : INotifyPropertyChanged
    {
        private string _name = "ok";

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
    }

    [WpfTheory]
    [InlineData("Wolfgang")]
    [InlineData("Feneberg")]
    public void Validate_Accepts_A_Nonempty_Value(string value)
    {
        var rule = new Ex045_NonEmptyValidationRule();

        var result = rule.Validate(value, CultureInfo.InvariantCulture);

        Assert.True(result.IsValid);
    }

    [WpfTheory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_Rejects_An_Empty_Or_Whitespace_Value_With_A_Required_Error(string value)
    {
        var rule = new Ex045_NonEmptyValidationRule();

        var result = rule.Validate(value, CultureInfo.InvariantCulture);

        Assert.False(result.IsValid);
        Assert.Equal("Required", result.ErrorContent);
    }

    [WpfFact]
    public void Bind_Wires_TwoWay_PropertyChanged_With_The_Rule_Attached()
    {
        var source = new NameSource();
        var target = new TextBox();

        Ex045_ValidationRules.Bind(target, source, nameof(NameSource.Name));

        var binding = BindingOperations.GetBinding(target, TextBox.TextProperty);
        Assert.NotNull(binding);
        Assert.Equal(nameof(NameSource.Name), binding!.Path.Path);
        Assert.Equal(BindingMode.TwoWay, binding.Mode);
        Assert.Equal(UpdateSourceTrigger.PropertyChanged, binding.UpdateSourceTrigger);
        Assert.Contains(binding.ValidationRules, r => r is Ex045_NonEmptyValidationRule);
    }

    [WpfFact]
    public void An_Invalid_Edit_Surfaces_A_Validation_Error_And_Never_Reaches_The_Source()
    {
        var source = new NameSource { Name = "Wolfgang" };
        var target = new TextBox();
        Ex045_ValidationRules.Bind(target, source, nameof(NameSource.Name));
        Layout(target);
        Pump();

        target.Text = "";
        Pump();

        Assert.True(Validation.GetHasError(target));
        var errors = Validation.GetErrors(target);
        Assert.Single(errors);
        Assert.Equal("Required", errors[0].ErrorContent);

        // The rule's whole point: a rejected value never gets pushed - the source keeps
        // what it had, the same "do not push" outcome row 017 reaches through UnsetValue.
        Assert.Equal("Wolfgang", source.Name);
    }

    [WpfFact]
    public void A_Later_Valid_Edit_Clears_The_Error_And_Reaches_The_Source()
    {
        var source = new NameSource { Name = "Wolfgang" };
        var target = new TextBox();
        Ex045_ValidationRules.Bind(target, source, nameof(NameSource.Name));
        Layout(target);
        Pump();

        target.Text = "";
        Pump();

        target.Text = "Feneberg";
        Pump();

        Assert.False(Validation.GetHasError(target));
        Assert.Equal("Feneberg", source.Name);
    }
}
