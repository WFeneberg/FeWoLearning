using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace FeWoLearning.Wpf.Tests;

/// <summary>
/// Exists so a broken harness fails loudly and first. If any of these five fail, every
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
        // Content = "..." is not incidental set-dressing - it is what makes this pass at
        // all. Content assignment acquires a logical child (AddLogicalChild), which flips
        // IsInitialized on both ends, and an unset default Style/Template never resolves
        // while IsInitialized is false. This test has been green since this track's first
        // exercise for that reason, not because Layout(...) alone is enough - see the
        // IsInitialized/AddLogicalChild finding in README.md and the sibling test below,
        // which proves the failure mode this one has always dodged.
        var button = new Button { Content = "Measure me" };

        Layout(button);

        Assert.True(button.IsInitialized, "Content assignment should have flipped IsInitialized.");
        Assert.NotNull(button.Template);
        Assert.True(button.DesiredSize.Width > 0, "A templated Button must measure wider than 0.");
        Assert.True(button.DesiredSize.Height > 0, "A templated Button must measure taller than 0.");
    }

    [WpfFact]
    public void A_Never_Initialized_Button_Never_Resolves_A_Template()
    {
        // The failure mode the test above has always avoided by setting Content. Nothing
        // here ever acquires a logical child or has Content assigned, so IsInitialized
        // stays false and the default template never resolves - Layout(...) does not help,
        // and no exception is thrown anywhere; the button just silently measures (0,0).
        // CompleteInitialization(...) is what rows 032-034 use to avoid exactly this.
        var button = new Button();

        Layout(button);

        Assert.False(button.IsInitialized);
        Assert.Null(button.Template);
        Assert.Equal(0.0, button.DesiredSize.Width);
        Assert.Equal(0.0, button.DesiredSize.Height);

        CompleteInitialization(button);
        Layout(button);

        Assert.True(button.IsInitialized);
        Assert.NotNull(button.Template);
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

        Show(panel);

        Assert.True(loaded, "Show(...) must connect the element to a PresentationSource, which is what raises Loaded.");
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
