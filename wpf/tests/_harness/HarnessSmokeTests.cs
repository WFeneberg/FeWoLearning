using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace FeWoLearning.Wpf.Tests;

/// <summary>
/// Exists so a broken harness fails loudly and first. If any of these four fail, every
/// other failure in the run is noise - fix the harness before reading anything else.
/// </summary>
public class HarnessSmokeTests : WpfTestContext
{
    [WpfFact]
    public void Runs_On_An_Sta_Thread_With_A_Dispatcher()
    {
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        Assert.NotNull(Dispatcher.CurrentDispatcher);
    }

    [WpfFact]
    public void Default_Control_Template_Resolves_Without_An_Application()
    {
        var button = new Button { Content = "Measure me" };

        Layout(button);

        Assert.NotNull(button.Template);
        Assert.True(button.DesiredSize.Width > 0, "A templated Button must measure wider than 0.");
        Assert.True(button.DesiredSize.Height > 0, "A templated Button must measure taller than 0.");
    }

    [WpfFact]
    public void Binding_Pushes_To_The_Target_After_Pumping()
    {
        var source = new SmokeSource { Text = "before" };
        var target = new TextBlock { DataContext = source };
        target.SetBinding(TextBlock.TextProperty, new Binding(nameof(SmokeSource.Text)));

        Layout(target);
        Assert.Equal("before", target.Text);

        source.Text = "after";
        Pump(DispatcherPriority.DataBind);

        Assert.Equal("after", target.Text);
    }

    [WpfFact]
    public void Hosted_Element_Raises_Loaded()
    {
        var loaded = false;
        var panel = new StackPanel();
        panel.Loaded += (_, _) => loaded = true;

        Host(panel);

        Assert.True(loaded, "Host(...) must connect the element to a PresentationSource, which is what raises Loaded.");
    }

    private sealed class SmokeSource : INotifyPropertyChanged
    {
        private string _text = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }
    }
}
