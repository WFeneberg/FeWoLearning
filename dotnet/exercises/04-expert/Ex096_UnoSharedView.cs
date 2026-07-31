using System.ComponentModel;

namespace FeWoLearning.Exercises.Expert;

// Exercise 096 — Uno Platform shared view logic (expert).
// Goal:   Model the platform-agnostic ViewModel layer Uno apps share across
//         WASM/Android/iOS/Windows heads: bindable display properties derived
//         from raw device state, with fine-grained INotifyPropertyChanged
//         notifications (only properties whose *value* actually changed fire,
//         which is what keeps shared-view XAML bindings from re-rendering
//         needlessly on every head).
// Drills: INotifyPropertyChanged, computed/derived bindable properties,
//         change-detection, deterministic precedence rules, guard clauses.
public enum ConnectionQuality
{
    Offline,
    Poor,
    Good,
    Excellent,
}

public sealed class UnoSharedView : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public double BatteryLevel { get; private set; }
    public bool IsCharging { get; private set; }
    public ConnectionQuality Connection { get; private set; }

    public string BatteryDisplayText { get; private set; } = string.Empty;
    public string StatusBadge { get; private set; } = string.Empty;
    public string StatusColorHex { get; private set; } = string.Empty;
    public bool IsCriticalAlert { get; private set; }

    // Recomputes every bindable property from the given raw inputs and raises
    // PropertyChanged only for properties whose value actually changed.
    //
    // Precedence for StatusBadge (first match wins):
    //   1. BatteryLevel <= 15 && !IsCharging  -> "Critical"
    //   2. Connection == Offline              -> "Offline"
    //   3. BatteryLevel <= 30 && !IsCharging  -> "Low"
    //   4. Connection == Poor                 -> "Weak Signal"
    //   5. otherwise                          -> "Normal"
    //
    // StatusColorHex per badge: Critical=#D32F2F, Offline=#616161, Low=#F57C00,
    // "Weak Signal"=#FBC02D, Normal=#388E3C.
    // IsCriticalAlert is true iff StatusBadge is "Critical" or "Offline".
    // BatteryDisplayText is "{level:0}%" with " (Charging)" appended when charging.
    //
    // Throws ArgumentOutOfRangeException if batteryLevel is outside [0, 100].
    public void Update(double batteryLevel, bool isCharging, ConnectionQuality connection)
        => throw new NotImplementedException();
}
