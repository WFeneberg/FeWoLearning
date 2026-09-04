using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex019_
public partial class Ex019_ButtonClickEvent : UserControl
{
    public Ex019_ButtonClickEvent()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex019 - wire EventButton.Click to a handler that bumps " +
            "EventClickCount, and CommandButton.Command to CommandClickCommand");
    }

    // TODO: Ex019 - implement this handler to increment
    // ((Ex019_ButtonClickEventViewModel)DataContext!).EventClickCount, then wire
    // it as EventButton's Click in the XAML above. Left unimplemented here so the
    // stub still compiles even before the XAML references it.
}

/// <summary>Given. Do not change.</summary>
public class Ex019_ButtonClickEventViewModel : ReactiveObject
{
    private int _eventClickCount;
    public int EventClickCount
    {
        get => _eventClickCount;
        set => this.RaiseAndSetIfChanged(ref _eventClickCount, value);
    }

    private int _commandClickCount;
    public int CommandClickCount
    {
        get => _commandClickCount;
        set => this.RaiseAndSetIfChanged(ref _commandClickCount, value);
    }

    public ReactiveCommand<RxVoid, RxVoid> CommandClickCommand { get; }

    public Ex019_ButtonClickEventViewModel()
    {
        CommandClickCommand = ReactiveCommand.Create(() => { CommandClickCount++; });
    }
}
