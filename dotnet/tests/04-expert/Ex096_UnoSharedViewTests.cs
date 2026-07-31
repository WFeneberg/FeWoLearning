using System;
using System.Collections.Generic;
using System.ComponentModel;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex096_UnoSharedViewTests
{
    [Fact]
    public void Update_NormalState_ComputesExpectedDisplayValues()
    {
        var view = new UnoSharedView();
        view.Update(80, false, ConnectionQuality.Good);

        Assert.Equal("80%", view.BatteryDisplayText);
        Assert.Equal("Normal", view.StatusBadge);
        Assert.Equal("#388E3C", view.StatusColorHex);
        Assert.False(view.IsCriticalAlert);
    }

    [Fact]
    public void Update_LowBatteryNotCharging_IsCritical()
    {
        var view = new UnoSharedView();
        view.Update(10, false, ConnectionQuality.Good);

        Assert.Equal("10%", view.BatteryDisplayText);
        Assert.Equal("Critical", view.StatusBadge);
        Assert.Equal("#D32F2F", view.StatusColorHex);
        Assert.True(view.IsCriticalAlert);
    }

    [Fact]
    public void Update_LowBatteryWhileCharging_IsNotCritical()
    {
        var view = new UnoSharedView();
        view.Update(10, true, ConnectionQuality.Good);

        Assert.Equal("10% (Charging)", view.BatteryDisplayText);
        Assert.Equal("Normal", view.StatusBadge);
        Assert.Equal("#388E3C", view.StatusColorHex);
        Assert.False(view.IsCriticalAlert);
    }

    [Fact]
    public void Update_OfflineConnectionOverridesModerateBattery()
    {
        var view = new UnoSharedView();
        view.Update(50, false, ConnectionQuality.Offline);

        Assert.Equal("50%", view.BatteryDisplayText);
        Assert.Equal("Offline", view.StatusBadge);
        Assert.Equal("#616161", view.StatusColorHex);
        Assert.True(view.IsCriticalAlert);
    }

    [Fact]
    public void Update_ModerateLowBatteryNotCharging_IsLow()
    {
        var view = new UnoSharedView();
        view.Update(25, false, ConnectionQuality.Poor);

        Assert.Equal("Low", view.StatusBadge);
        Assert.Equal("#F57C00", view.StatusColorHex);
        Assert.False(view.IsCriticalAlert);
    }

    [Fact]
    public void Update_PoorConnectionWithHealthyBattery_IsWeakSignal()
    {
        var view = new UnoSharedView();
        view.Update(50, false, ConnectionQuality.Poor);

        Assert.Equal("Weak Signal", view.StatusBadge);
        Assert.Equal("#FBC02D", view.StatusColorHex);
        Assert.False(view.IsCriticalAlert);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100.1)]
    public void Update_BatteryLevelOutOfRange_Throws(double level)
    {
        var view = new UnoSharedView();
        Assert.Throws<ArgumentOutOfRangeException>(() => view.Update(level, false, ConnectionQuality.Good));
    }

    [Fact]
    public void Update_RepeatedIdenticalCall_RaisesNoPropertyChanged()
    {
        var view = new UnoSharedView();
        view.Update(80, false, ConnectionQuality.Good);

        var raised = new List<string>();
        view.PropertyChanged += (_, e) => raised.Add(e.PropertyName!);

        view.Update(80, false, ConnectionQuality.Good);

        Assert.Empty(raised);
    }

    [Fact]
    public void Update_OnlyChargingFlagChanges_RaisesOnlyAffectedProperties()
    {
        var view = new UnoSharedView();
        view.Update(80, false, ConnectionQuality.Good);

        var raised = new HashSet<string>();
        view.PropertyChanged += (_, e) => raised.Add(e.PropertyName!);

        view.Update(80, true, ConnectionQuality.Good);

        Assert.Equal(new HashSet<string> { nameof(UnoSharedView.IsCharging), nameof(UnoSharedView.BatteryDisplayText) }, raised);
    }

    [Fact]
    public void Update_FirstCall_RaisesChangedComputedProperties()
    {
        // Every UnoSharedView instance starts at the CLR defaults (0, false,
        // ConnectionQuality.Offline, empty strings). IsCharging is not included
        // below because `false` is both the default *and* the value passed in,
        // so that property never actually changes and must not raise.
        var view = new UnoSharedView();
        var raised = new HashSet<string>();
        view.PropertyChanged += (_, e) => raised.Add(e.PropertyName!);

        view.Update(5, false, ConnectionQuality.Poor);

        var expected = new HashSet<string>
        {
            nameof(UnoSharedView.BatteryLevel),
            nameof(UnoSharedView.Connection),
            nameof(UnoSharedView.BatteryDisplayText),
            nameof(UnoSharedView.StatusBadge),
            nameof(UnoSharedView.StatusColorHex),
            nameof(UnoSharedView.IsCriticalAlert),
        };
        Assert.Equal(expected, raised);
    }
}
