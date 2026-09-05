using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex012_LegacyEventToInpcTests : WpfTestContext
{
    [WpfFact]
    public void A_Real_Binding_Refreshes_After_Volume_Changes()
    {
        var model = new Ex012_VolumeControl { Volume = 10 };
        var target = new TextBlock { DataContext = model };
        target.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex012_VolumeControl.Volume)));
        Layout(target);
        Pump();

        Assert.Equal("10", target.Text);

        model.Volume = 75;
        Pump();

        // This is the load-bearing assertion: a real Binding only reacts to
        // INotifyPropertyChanged. If the setter only raised the legacy VolumeChanged
        // event, this target would still read "10" here - there is no way to satisfy
        // this without actually implementing PropertyChanged.
        Assert.Equal("75", target.Text);
    }

    [WpfFact]
    public void The_Legacy_VolumeChanged_Event_Still_Fires_For_Old_Subscribers()
    {
        var model = new Ex012_VolumeControl { Volume = 10 };
        var raised = 0;
        EventHandler handler = (_, _) => raised++;
        model.VolumeChanged += handler;

        model.Volume = 20;

        Assert.Equal(1, raised);
    }

    [WpfFact]
    public void PropertyChanged_Names_Volume()
    {
        var model = new Ex012_VolumeControl { Volume = 10 };
        var names = new List<string?>();
        model.PropertyChanged += (_, e) => names.Add(e.PropertyName);

        model.Volume = 20;

        Assert.Equal(new string?[] { "Volume" }, names);
    }

    [WpfFact]
    public void Assigning_An_Equal_Value_Raises_Neither_Event()
    {
        var model = new Ex012_VolumeControl { Volume = 10 };
        var propertyChangedCount = 0;
        var volumeChangedCount = 0;
        model.PropertyChanged += (_, _) => propertyChangedCount++;
        model.VolumeChanged += (_, _) => volumeChangedCount++;

        model.Volume = 10;

        Assert.Equal(0, propertyChangedCount);
        Assert.Equal(0, volumeChangedCount);
    }
}
