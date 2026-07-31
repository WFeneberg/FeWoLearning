using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Exercises.Expert;

// Exercise 096 — Uno Platform shared view logic (reference solution).
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

    private double _batteryLevel;
    private bool _isCharging;
    private ConnectionQuality _connection;
    private string _batteryDisplayText = string.Empty;
    private string _statusBadge = string.Empty;
    private string _statusColorHex = string.Empty;
    private bool _isCriticalAlert;

    public double BatteryLevel => _batteryLevel;
    public bool IsCharging => _isCharging;
    public ConnectionQuality Connection => _connection;
    public string BatteryDisplayText => _batteryDisplayText;
    public string StatusBadge => _statusBadge;
    public string StatusColorHex => _statusColorHex;
    public bool IsCriticalAlert => _isCriticalAlert;

    public void Update(double batteryLevel, bool isCharging, ConnectionQuality connection)
    {
        if (batteryLevel < 0 || batteryLevel > 100)
            throw new ArgumentOutOfRangeException(nameof(batteryLevel), batteryLevel, "Must be between 0 and 100.");

        string badge = batteryLevel <= 15 && !isCharging
            ? "Critical"
            : connection == ConnectionQuality.Offline
                ? "Offline"
                : batteryLevel <= 30 && !isCharging
                    ? "Low"
                    : connection == ConnectionQuality.Poor
                        ? "Weak Signal"
                        : "Normal";

        string colorHex = badge switch
        {
            "Critical" => "#D32F2F",
            "Offline" => "#616161",
            "Low" => "#F57C00",
            "Weak Signal" => "#FBC02D",
            _ => "#388E3C",
        };

        bool criticalAlert = badge is "Critical" or "Offline";

        string displayText = isCharging
            ? $"{batteryLevel:0}% (Charging)"
            : $"{batteryLevel:0}%";

        SetField(ref _batteryLevel, batteryLevel, nameof(BatteryLevel));
        SetField(ref _isCharging, isCharging, nameof(IsCharging));
        SetField(ref _connection, connection, nameof(Connection));
        SetField(ref _batteryDisplayText, displayText, nameof(BatteryDisplayText));
        SetField(ref _statusBadge, badge, nameof(StatusBadge));
        SetField(ref _statusColorHex, colorHex, nameof(StatusColorHex));
        SetField(ref _isCriticalAlert, criticalAlert, nameof(IsCriticalAlert));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
