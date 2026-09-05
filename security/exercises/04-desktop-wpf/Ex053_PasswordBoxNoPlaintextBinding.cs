using System.Windows.Controls;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 053 — PasswordBoxNoPlaintextBinding (desktop-wpf).
// Goal:   PasswordBox.Password is deliberately NOT a DependencyProperty (Microsoft
//         will not let a plaintext password sit in the binding/property-value cache
//         where a debugger, a binding-trace, or a rogue value converter could read
//         it), so it cannot be data-bound with {Binding Password}. Attach must wire
//         the box up in code instead, and the view model it feeds must never carry
//         the password anywhere a caller could read it back out.
// Drills: PasswordBox, why Password is not a DependencyProperty.
// Passes: attack facts   - graded by reflection, not behaviour: Ex053_LoginViewModel's
//                          public properties AND fields contain no member of type
//                          `string` whose name contains "password" (case-insensitive),
//                          so a solution that simply adds a plaintext property to
//                          satisfy the compiler fails this check even though it never
//                          reads that property back; its public surface also exposes
//                          no member of type PasswordBox. State this plainly for the
//                          learner: the grader inspects metadata via
//                          System.Reflection, it does not merely watch behaviour.
//         use facts      - after Attach(box, viewModel), setting box.Password to a
//                          non-empty value and pumping the dispatcher flips CanSubmit
//                          from false to true; clearing box.Password back to ""
//                          and pumping flips CanSubmit back to false.
public sealed class Ex053_LoginViewModel
{
    public Ex053_LoginViewModel() =>
        throw new NotImplementedException(
            "TODO: Ex053 - store only whether a password is currently present, never the characters themselves");

    public bool CanSubmit =>
        throw new NotImplementedException(
            "TODO: Ex053 - true once a non-empty password has been reported, false once it is cleared");

    // Internal, not public: Attach may call this to report presence/absence of a
    // password without ever handing the view model the characters themselves.
    internal void UpdatePasswordPresence(bool hasPassword) =>
        throw new NotImplementedException(
            "TODO: Ex053 - update CanSubmit from hasPassword; never store the password text anywhere");
}

public static class Ex053_PasswordBoxNoPlaintextBinding
{
    public static void Attach(PasswordBox box, Ex053_LoginViewModel viewModel) =>
        throw new NotImplementedException(
            "TODO: Ex053 - subscribe to box.PasswordChanged and forward only whether box.Password is non-empty " +
            "to the view model (PasswordBox.Password is not a DependencyProperty, so {Binding} cannot reach it)");
}
