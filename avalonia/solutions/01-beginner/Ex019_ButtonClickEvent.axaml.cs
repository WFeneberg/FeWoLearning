using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex019_
public partial class Ex019_ButtonClickEvent : UserControl
{
    public Ex019_ButtonClickEvent() => InitializeComponent();

    private void OnEventButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is Ex019_ButtonClickEventViewModel vm) vm.EventClickCount++;
    }
}

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
