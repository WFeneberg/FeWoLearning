using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex043_NotifyDataErrorInfoTests : WpfTestContext
{
    // Exposes SetErrors directly for the structural tests below, decoupled from any one
    // property's own validation logic. Also carries a plain, unvalidated Name property (its
    // own setter never calls SetErrors) so a test can bind a target to something real while
    // setting errors purely programmatically, with no target edit anywhere.
    private sealed class TwoFieldProbe : Ex043_ValidatingViewModelBase
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

        public void SetErrorsFor(string propertyName, params string[] errors) => SetErrors(propertyName, errors);

        public void SetErrorsFor(string propertyName, IReadOnlyList<string> errors) => SetErrors(propertyName, errors);
    }

    // A real validated property, for the end-to-end binding test - this is what a concrete
    // validating view model looks like; deliberately not shipped in the content library.
    private sealed class NameProbe : Ex043_ValidatingViewModelBase
    {
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
                SetErrors(nameof(Name), string.IsNullOrWhiteSpace(value) ? ["Required"] : []);
            }
        }
    }

    [WpfFact]
    public void A_Fresh_Probe_Has_No_Errors_Anywhere()
    {
        INotifyDataErrorInfo probe = new TwoFieldProbe();

        Assert.False(probe.HasErrors);
        Assert.Empty(probe.GetErrors("Name"));
        Assert.Empty(probe.GetErrors(null));
    }

    [WpfFact]
    public void SetErrors_Records_Per_Property_Errors_Retrievable_Through_The_Interface()
    {
        var probe = new TwoFieldProbe();
        probe.SetErrorsFor("Name", "Required");

        // Reached through the INotifyDataErrorInfo reference, not the concrete class - a
        // side-channel property/event instead of the real interface members would not
        // satisfy this.
        INotifyDataErrorInfo iface = probe;
        Assert.Contains("Required", iface.GetErrors("Name").Cast<string>());
        Assert.Empty(iface.GetErrors("Age"));
    }

    [WpfFact]
    public void SetErrors_With_A_Nonempty_List_Raises_ErrorsChanged_And_Flips_HasErrors()
    {
        var probe = new TwoFieldProbe();
        INotifyDataErrorInfo iface = probe;
        var errorsChangedFor = new List<string?>();
        var hasErrorsRaises = 0;

        iface.ErrorsChanged += (_, e) => errorsChangedFor.Add(e.PropertyName);
        probe.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(INotifyDataErrorInfo.HasErrors)) hasErrorsRaises++;
        };

        probe.SetErrorsFor("Name", "Required");

        Assert.Equal(new string?[] { "Name" }, errorsChangedFor);
        Assert.Equal(1, hasErrorsRaises);
        Assert.True(iface.HasErrors);
    }

    [WpfFact]
    public void SetErrors_With_The_Same_List_Again_Raises_Nothing()
    {
        var probe = new TwoFieldProbe();
        probe.SetErrorsFor("Name", "Required");

        var errorsChangedCount = 0;
        var propertyChangedCount = 0;
        ((INotifyDataErrorInfo)probe).ErrorsChanged += (_, _) => errorsChangedCount++;
        probe.PropertyChanged += (_, _) => propertyChangedCount++;

        probe.SetErrorsFor("Name", "Required"); // logically identical set, new array instance

        Assert.Equal(0, errorsChangedCount);
        Assert.Equal(0, propertyChangedCount);
    }

    [WpfFact]
    public void Clearing_Errors_Raises_ErrorsChanged_And_Flips_HasErrors_Back()
    {
        var probe = new TwoFieldProbe();
        probe.SetErrorsFor("Name", "Required");

        var errorsChangedCount = 0;
        var hasErrorsRaises = 0;
        ((INotifyDataErrorInfo)probe).ErrorsChanged += (_, _) => errorsChangedCount++;
        probe.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(INotifyDataErrorInfo.HasErrors)) hasErrorsRaises++;
        };

        probe.SetErrorsFor("Name"); // empty - clears it

        Assert.Equal(1, errorsChangedCount);
        Assert.Equal(1, hasErrorsRaises);
        Assert.False(((INotifyDataErrorInfo)probe).HasErrors);
        Assert.Empty(((INotifyDataErrorInfo)probe).GetErrors("Name"));
    }

    [WpfFact]
    public void HasErrors_Reflects_Multiple_Properties_Independently()
    {
        var probe = new TwoFieldProbe();
        INotifyDataErrorInfo iface = probe;

        probe.SetErrorsFor("Name", "Required");
        probe.SetErrorsFor("Age", "Must be positive");
        Assert.True(iface.HasErrors);

        // Load-bearing against a HasErrors computed independently of the actual error store
        // (e.g. a bool flipped true once and never re-derived): clearing ONE property's
        // errors while the OTHER still has one must leave HasErrors true.
        probe.SetErrorsFor("Name");
        Assert.True(iface.HasErrors);

        probe.SetErrorsFor("Age");
        Assert.False(iface.HasErrors);
    }

    [WpfFact]
    public void An_Error_Set_With_No_Target_Edit_Still_Reaches_A_Bound_Target_Through_ErrorsChanged()
    {
        var probe = new TwoFieldProbe { Name = "Wolfgang" };
        var target = new TextBox();
        target.SetBinding(TextBox.TextProperty, new Binding(nameof(TwoFieldProbe.Name)) { Source = probe });
        Layout(target);
        Pump();

        // No target edit anywhere - SetErrors runs purely programmatically. This is the
        // scenario that actually needs ErrorsChanged: an implementation that raises it
        // BEFORE mutating the store lets every subscriber (WPF's own BindingExpression
        // included) re-query the PREVIOUS set, so the target would still read no error here;
        // an implementation that never raises it at all would never revalidate either.
        probe.SetErrorsFor("Name", "Required");
        Pump();

        Assert.True(Validation.GetHasError(target));

        probe.SetErrorsFor("Name");
        Pump();

        Assert.False(Validation.GetHasError(target));
    }

    [WpfFact]
    public void SetErrors_Stores_Its_Own_Copy_Not_A_Reference_To_The_Callers_List()
    {
        var probe = new TwoFieldProbe();
        var callerOwned = new List<string> { "Required" };

        probe.SetErrorsFor("Name", callerOwned);

        // Mutating the caller's own list AFTER the call must not be visible through the
        // probe - an aliasing SetErrors that stores the reference verbatim would leak this.
        callerOwned.Add("Also too long");
        callerOwned[0] = "Changed";

        Assert.Equal(new[] { "Required" }, ((INotifyDataErrorInfo)probe).GetErrors("Name").Cast<string>());
    }

    [WpfFact]
    public void A_Bound_TextBox_Shows_A_Validation_Error_With_No_Flag_Set_Anywhere()
    {
        var source = new NameProbe { Name = "Wolfgang" };
        var target = new TextBox { DataContext = source };
        target.SetBinding(TextBox.TextProperty, new Binding(nameof(NameProbe.Name))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            // Deliberately NOT setting ValidatesOnNotifyDataErrors - measured to default true.
        });
        Layout(target);
        Pump();

        target.Text = "";
        Pump();

        Assert.True(Validation.GetHasError(target));

        target.Text = "Feneberg";
        Pump();

        Assert.False(Validation.GetHasError(target));
    }
}
