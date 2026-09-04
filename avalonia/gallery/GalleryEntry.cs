using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Gallery;

/// <param name="Id">Three-digit exercise number, e.g. "001".</param>
/// <param name="Title">Exercise slug, e.g. "HelloView".</param>
/// <param name="Create">Builds the demo page. In exercises mode this throws the
/// exercise's NotImplementedException, which is the correct behaviour.</param>
public sealed record GalleryEntry(string Id, string Title, Func<Control> Create);
