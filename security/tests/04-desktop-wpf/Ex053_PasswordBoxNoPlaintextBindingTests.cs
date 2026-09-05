using System.Reflection;
using System.Windows.Controls;
using FeWoLearning.Security.Exercises.DesktopWpf;
using FeWoLearning.Security.Tests.Harness;

namespace FeWoLearning.Security.Tests.DesktopWpf;

public class Ex053_PasswordBoxNoPlaintextBindingTests
{
    [WpfFact]
    public void Attack_ViewModel_Exposes_No_Public_String_Member_Named_Password()
    {
        // Constructing the view model is itself part of the red run: the stub's
        // constructor throws NotImplementedException, so this fails there for the
        // right reason before the reflection assertions below are ever reached.
        var viewModel = new Ex053_LoginViewModel();
        var type = viewModel.GetType();

        var offendingProperties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(string) &&
                        p.Name.Contains("password", StringComparison.OrdinalIgnoreCase));
        var offendingFields = type
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.FieldType == typeof(string) &&
                        f.Name.Contains("password", StringComparison.OrdinalIgnoreCase));

        Assert.Empty(offendingProperties);
        Assert.Empty(offendingFields);
    }

    [WpfFact]
    public void Attack_ViewModel_Exposes_No_Public_PasswordBox_Member()
    {
        var viewModel = new Ex053_LoginViewModel();
        var type = viewModel.GetType();

        var offendingProperties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(PasswordBox));
        var offendingFields = type
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.FieldType == typeof(PasswordBox));

        Assert.Empty(offendingProperties);
        Assert.Empty(offendingFields);
    }

    [WpfFact]
    public void Use_Setting_The_Password_Flips_CanSubmit_From_False_To_True()
    {
        var box = new PasswordBox();
        var viewModel = new Ex053_LoginViewModel();
        Ex053_PasswordBoxNoPlaintextBinding.Attach(box, viewModel);

        Assert.False(viewModel.CanSubmit);

        box.Password = "hunter2";
        WpfPump.Pump();

        Assert.True(viewModel.CanSubmit);
    }

    [WpfFact]
    public void Use_Clearing_The_Password_Flips_CanSubmit_Back_To_False()
    {
        var box = new PasswordBox();
        var viewModel = new Ex053_LoginViewModel();
        Ex053_PasswordBoxNoPlaintextBinding.Attach(box, viewModel);

        box.Password = "hunter2";
        WpfPump.Pump();
        Assert.True(viewModel.CanSubmit);

        box.Password = "";
        WpfPump.Pump();

        Assert.False(viewModel.CanSubmit);
    }
}
