using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex010_
public partial class Ex010_CompiledBinding : UserControl
{
    public Ex010_CompiledBinding()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex010 - bind Title and Author.Name with {CompiledBinding}");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex010_BookViewModel : ReactiveObject
{
    private string _title = "";
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private Ex010_AuthorViewModel _author = new();
    public Ex010_AuthorViewModel Author
    {
        get => _author;
        set => this.RaiseAndSetIfChanged(ref _author, value);
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex010_AuthorViewModel : ReactiveObject
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}
