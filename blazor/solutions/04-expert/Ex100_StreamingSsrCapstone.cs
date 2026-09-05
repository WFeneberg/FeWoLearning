using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 100 - Streaming SSR Capstone (expert).
/// Goal:   The shape streaming SSR asks a component for - render something at once,
///         then replace it when the data arrives - built by hand out of everything
///         this tier covered.
/// Drills: [StreamRendering], a hand-written BuildRenderTree with two states, keys
///         chosen so the second state REPLACES the first rather than being patched
///         into it, and the async initialization that drives the switch.
/// Passes: dotnet test --filter FullyQualifiedName~Ex100_
/// </summary>
/// <remarks>
/// bUnit cannot stream, and this track does not pretend otherwise (README section 7).
/// What it can show is the whole component-side contract: the first render happens
/// before the data is there, the second one arrives when it is, and the swap is a
/// replacement. The [StreamRendering] attribute is asserted from metadata, the same
/// documented exception ex069 makes.
/// </remarks>
[StreamRendering]
public class Ex100_StreamingSsrCapstone : ComponentBase
{
    public const string LoadingKey = "loading";
    public const string LoadedKey = "loaded";

    [Parameter] public Func<Task<IReadOnlyList<string>>> Load { get; set; }
        = () => Task.FromResult<IReadOnlyList<string>>([]);

    public bool IsLoading { get; private set; } = true;

    public IReadOnlyList<string> Items { get; private set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Items = await Load();
        IsLoading = false;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IsLoading)
        {
            builder.OpenElement(0, "p");
            builder.AddAttribute(1, "class", "placeholder");
            builder.AddContent(2, "loading…");
            builder.CloseElement();
        }
        else
        {
            builder.OpenElement(3, "ul");
            builder.AddAttribute(4, "class", "items");

            foreach (var item in Items)
            {
                builder.OpenElement(5, "li");
                builder.AddContent(6, item);
                builder.CloseElement();
            }

            builder.CloseElement();
        }

        // Same sequence number in both branches, so the diff would happily keep one
        // panel - and different keys, which override that and make it a replacement.
        builder.OpenComponent<Ex100_StreamingSsrCapstone_Panel>(10);
        builder.SetKey(IsLoading ? LoadingKey : LoadedKey);
        builder.AddComponentParameter(
            11,
            nameof(Ex100_StreamingSsrCapstone_Panel.State),
            IsLoading ? LoadingKey : LoadedKey);
        builder.CloseComponent();
    }
}
