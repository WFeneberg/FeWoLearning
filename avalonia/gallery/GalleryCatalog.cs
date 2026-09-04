namespace FeWoLearning.Avalonia.Gallery;

public static class GalleryCatalog
{
    /// <summary>
    /// One entry per exercise whose result is visual. View-model-only exercises
    /// (ex008, ex009) deliberately have no page.
    /// </summary>
    public static IReadOnlyList<GalleryEntry> Entries { get; } = [];
}
