using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex087_
public static class Ex087_ResourceDictionaryMerging
{
    /// <summary>Given. Do not change.</summary>
    public const string Contested = "Contested";

    /// <summary>Given. Do not change.</summary>
    public const string OnlyInBase = "OnlyInBase";

    /// <summary>Given. Do not change.</summary>
    public const string OnlyInOverlay = "OnlyInOverlay";

    /// <summary>Given. Do not change.</summary>
    public const string OnlyOnHost = "OnlyOnHost";

    public static Control BuildHost()
    {
        var host = new StackPanel();

        // Added first, so lowest priority among the merged pair.
        host.Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            [Contested] = "base",
            [OnlyInBase] = "base",
        });

        // Added last, so it wins over the one above.
        host.Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            [Contested] = "overlay",
            [OnlyInOverlay] = "overlay",
        });

        // The host's own entries beat every merged dictionary.
        host.Resources[Contested] = "host";
        host.Resources[OnlyOnHost] = "host-only";

        var consumer = new Border { Name = "Consumer" };
        consumer.Bind(Border.TagProperty, new DynamicResourceExtension(Contested));
        host.Children.Add(consumer);

        return host;
    }
}
