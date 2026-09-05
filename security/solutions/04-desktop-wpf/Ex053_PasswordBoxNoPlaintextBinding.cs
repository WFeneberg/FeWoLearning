using System.Windows.Controls;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 053 — PasswordBoxNoPlaintextBinding (reference solution).
public sealed class Ex053_LoginViewModel
{
    private bool _hasPassword;

    public bool CanSubmit => _hasPassword;

    internal void UpdatePasswordPresence(bool hasPassword) => _hasPassword = hasPassword;
}

public static class Ex053_PasswordBoxNoPlaintextBinding
{
    public static void Attach(PasswordBox box, Ex053_LoginViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(box);
        ArgumentNullException.ThrowIfNull(viewModel);

        viewModel.UpdatePasswordPresence(box.Password.Length > 0);
        box.PasswordChanged += (_, _) => viewModel.UpdatePasswordPresence(box.Password.Length > 0);
    }
}
