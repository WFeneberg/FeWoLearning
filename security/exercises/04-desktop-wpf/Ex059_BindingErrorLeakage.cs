using System.Windows.Controls;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 059 — BindingErrorLeakage (desktop-wpf).
// Goal:   A data binding whose path does not resolve is a failure with an audience:
//         the exception, the source's runtime type name, and the property path it
//         tried to read are all things an attacker probing a shared or screen-shared
//         desktop app should never see rendered into a control's text or tooltip.
//         Bind must configure a live, real binding — not a one-time snapshot read —
//         that falls back to a caller-supplied, generic string on failure, and must
//         never route the underlying exception or path into anything the UI shows.
// Drills: binding failure surfaces, tooltips and traces as leak channels.
// Passes: attack facts   - when `path` does not exist on `source`, target.Text equals
//                          `fallback` and contains neither the source's type name nor
//                          the literal path; target.ToolTip is either null or, if set,
//                          contains neither of those either.
//         use facts      - when `path` does resolve, target.Text shows the current
//                          value; and after the source is mutated (it implements
//                          INotifyPropertyChanged) and WpfPump.Pump(DispatcherPriority.DataBind)
//                          is called, target.Text follows the new value. That last
//                          fact is the one that rules out reading the property once
//                          and assigning a plain string instead of a real binding.
public static class Ex059_BindingErrorLeakage
{
    public static void Bind(TextBlock target, object source, string path, string fallback) =>
        throw new NotImplementedException(
            "TODO: Ex059 - SetBinding(TextBlock.TextProperty, new Binding(path) { Source = source, " +
            "FallbackValue = fallback }); never surface the failed path or the source's type name to the user");
}
