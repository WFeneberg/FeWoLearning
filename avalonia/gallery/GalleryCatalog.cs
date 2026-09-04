using FeWoLearning.Avalonia.Gallery.Pages.Beginner;

namespace FeWoLearning.Avalonia.Gallery;

public static class GalleryCatalog
{
    /// <summary>
    /// One entry per exercise whose result is visual. View-model-only exercises
    /// (ex008, ex009) deliberately have no page.
    /// </summary>
    public static IReadOnlyList<GalleryEntry> Entries { get; } =
    [
        new("001", "HelloView", () => new Ex001()),
        new("002", "LayoutStackPanel", () => new Ex002()),
        new("003", "LayoutGrid", () => new Ex003()),
        new("004", "LayoutGridSpan", () => new Ex004()),
        new("005", "LayoutDockPanel", () => new Ex005()),
        new("006", "AlignmentAndMargin", () => new Ex006()),
        new("007", "LayoutWrapPanel", () => new Ex007()),
        new("010", "CompiledBinding", () => new Ex010()),
        new("011", "BindingModes", () => new Ex011()),
        new("012", "TextBoxTwoWay", () => new Ex012()),
        new("013", "BindingStringFormat", () => new Ex013()),
        new("014", "BindingFallback", () => new Ex014()),
        new("015", "ValueConverter", () => new Ex015()),
        new("016", "ReactiveCommandBasics", () => new Ex016()),
        new("017", "CommandCanExecute", () => new Ex017()),
        new("018", "CommandParameter", () => new Ex018()),
        new("019", "ButtonClickEvent", () => new Ex019()),
        new("020", "CheckBoxBinding", () => new Ex020()),
    ];
}
