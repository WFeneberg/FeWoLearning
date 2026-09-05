using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Architecture.Exercises.Desktop.Ex017;

/// <summary>
/// The base every view model in this exercise sits on. Note what it is NOT: it has no
/// reference to WPF, Avalonia, WinUI or anything else that draws. INotifyPropertyChanged
/// lives in System.ComponentModel, and a view model is testable precisely because that
/// is the only contract it needs.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Raise([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Assign <paramref name="value"/> to <paramref name="field"/> and raise
    /// PropertyChanged - but ONLY if the value actually changed. Returns whether it did.
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) =>
        throw new NotImplementedException(
            "TODO: Ex017 - assign and raise only when the value actually differs, and report whether it did");
}

// Exercise 017 — MvvmComposition (desktop).
// Goal:   Make a view model that notifies correctly - which means notifying when
//         something changed, staying silent when nothing did, and remembering the
//         properties that are computed from the ones being set.
// Drills: INotifyPropertyChanged, change detection, derived-property notification.
// Passes: CustomerName - setting a new value raises PropertyChanged once, naming it.
//         no-op set    - setting the SAME value raises nothing at all.
//         Quantity     - setting it raises BOTH "Quantity" and "Total".
//         Total        - recomputes from Quantity and UnitPrice.
//
// The derived-property notification is the bug every MVVM codebase has at least once.
// Total is correct the whole time; it is simply never re-read, because nobody told the
// binding it had changed. It looks like a rendering bug and it is a notification bug.
public sealed class OrderViewModel : ObservableObject
{
    private string _customerName = "";
    private int _quantity;
    private decimal _unitPrice;

    public string CustomerName
    {
        get => _customerName;
        set => throw new NotImplementedException("TODO: Ex017 - set through SetProperty");
    }

    public int Quantity
    {
        get => _quantity;
        set => throw new NotImplementedException(
            "TODO: Ex017 - set through SetProperty and, when it changed, also raise Total");
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set => throw new NotImplementedException(
            "TODO: Ex017 - set through SetProperty and, when it changed, also raise Total");
    }

    public decimal Total => Quantity * UnitPrice;
}
