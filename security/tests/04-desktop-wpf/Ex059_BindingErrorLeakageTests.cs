using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;
using FeWoLearning.Security.Exercises.DesktopWpf;
using FeWoLearning.Security.Tests.Harness;

namespace FeWoLearning.Security.Tests.DesktopWpf;

public class Ex059_BindingErrorLeakageTests
{
    private sealed class Person : INotifyPropertyChanged
    {
        private string _name = "";

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

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    [WpfFact]
    public void Attack_A_Missing_Path_Falls_Back_Without_Naming_The_Type_Or_Path()
    {
        var target = new TextBlock();
        var person = new Person();

        Ex059_BindingErrorLeakage.Bind(target, person, "DoesNotExist", "n/a");
        WpfPump.Pump(DispatcherPriority.DataBind);

        Assert.Equal("n/a", target.Text);
        Assert.DoesNotContain("Person", target.Text);
        Assert.DoesNotContain("DoesNotExist", target.Text);
    }

    [WpfFact]
    public void Attack_The_ToolTip_Is_Empty_Or_Free_Of_Type_And_Path_After_A_Binding_Failure()
    {
        var target = new TextBlock();
        var person = new Person();

        Ex059_BindingErrorLeakage.Bind(target, person, "DoesNotExist", "n/a");
        WpfPump.Pump(DispatcherPriority.DataBind);

        var toolTipText = target.ToolTip as string;
        Assert.True(
            toolTipText is null || (!toolTipText.Contains("Person") && !toolTipText.Contains("DoesNotExist")));
    }

    [WpfFact]
    public void Use_A_Resolving_Path_Shows_The_Value()
    {
        var target = new TextBlock();
        var person = new Person { Name = "Ada" };

        Ex059_BindingErrorLeakage.Bind(target, person, "Name", "n/a");
        WpfPump.Pump(DispatcherPriority.DataBind);

        Assert.Equal("Ada", target.Text);
    }

    [WpfFact]
    public void Use_Mutating_The_Source_And_Pumping_Updates_The_Bound_Text()
    {
        var target = new TextBlock();
        var person = new Person { Name = "Ada" };

        Ex059_BindingErrorLeakage.Bind(target, person, "Name", "n/a");
        WpfPump.Pump(DispatcherPriority.DataBind);
        Assert.Equal("Ada", target.Text);

        person.Name = "Grace";
        WpfPump.Pump(DispatcherPriority.DataBind);

        Assert.Equal("Grace", target.Text);
    }
}
