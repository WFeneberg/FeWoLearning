// Exercise 041 - View-model-first navigation (intermediate). REFERENCE SOLUTION.
// Goal:   Drive which "page" a shell displays purely by swapping an object reference on a
//         view model - never by constructing a view and assigning it to a ContentControl by
//         hand. A ContentControl bound to a CurrentViewModel property, plus a DataTemplate
//         keyed by the view model's own type, is enough for WPF to pick and instantiate the
//         matching view automatically whenever that reference changes.
// Drills: a CurrentViewModel property that raises PropertyChanged with the property system's
//         normal name, a DataTemplate registered under the *implicit content-template* key -
//         which, measured directly, is NOT the bare Type the way row 023's implicit Style key
//         is: it is System.Windows.DataTemplateKey(type). A DataTemplate keyed by the plain
//         Type compiles and adds to the dictionary without error, but is never found - the
//         ContentPresenter silently falls back to calling ToString() on the content object.
//         And a live, two-way-free Binding from ContentControl.Content to CurrentViewModel
//         (not a one-off assignment) - only a real Binding keeps the displayed page in sync
//         as CurrentViewModel keeps changing after the initial navigation.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Owns the single "current page" of a view-model-first shell.
/// </summary>
public sealed class Ex041_NavigationShell : INotifyPropertyChanged
{
    private object? _currentViewModel;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// The view model of whichever page is presently shown - any CLR object, typically a
    /// per-page view model instance. Assigning a NEW reference must raise PropertyChanged for
    /// this property; assigning the SAME reference again must raise nothing, the same
    /// "no event without a real change" rule every earlier INotifyPropertyChanged row in this
    /// track already follows.
    /// </summary>
    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set => AssignCurrentViewModel(ref _currentViewModel, value);
    }

    private void AssignCurrentViewModel(ref object? field, object? value)
    {
        if (ReferenceEquals(field, value))
        {
            return;
        }

        field = value;
        RaisePropertyChanged(nameof(CurrentViewModel));
    }

    /// <summary>Raises PropertyChanged. Ready to use.</summary>
    private void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public static class Ex041_ViewModelFirstNavigation
{
    /// <summary>
    /// Registers <paramref name="template"/> as the IMPLICIT content template for
    /// <paramref name="viewModelType"/> inside <paramref name="resources"/> - reachable only
    /// through the element tree (this harness has no Application, the same absence row 023
    /// depends on for implicit styles).
    /// </summary>
    public static void RegisterViewTemplate(ResourceDictionary resources, Type viewModelType, DataTemplate template)
        => resources[new DataTemplateKey(viewModelType)] = template;

    /// <summary>
    /// Wires <paramref name="host"/>.Content to follow <paramref name="shell"/>'s
    /// CurrentViewModel with a real, live Binding - not a single assignment taken at the
    /// moment this method runs.
    /// </summary>
    public static void BindShell(ContentControl host, Ex041_NavigationShell shell)
        => host.SetBinding(ContentControl.ContentProperty, new Binding(nameof(Ex041_NavigationShell.CurrentViewModel)) { Source = shell });
}
