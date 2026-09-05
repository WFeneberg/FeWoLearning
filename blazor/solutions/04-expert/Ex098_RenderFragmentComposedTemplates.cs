using FeWoLearning.Blazor.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 098 - Render Fragment Composed Templates (expert).
/// Goal:   The same composition as ex097, one type parameter up - functions that
///         take a template and give you back another template.
/// Drills: RenderFragment&lt;T&gt; as Func&lt;T, RenderFragment&gt;, decorating a
///         template, adapting the value on its way in, and turning a template plus a
///         sequence into a plain fragment.
/// Passes: dotnet test --filter FullyQualifiedName~Ex098_
/// </summary>
public class Ex098_RenderFragmentComposedTemplates : ComponentBase
{
    [Parameter] public IReadOnlyList<Person> People { get; set; } = [];

    public static RenderFragment<string> Label()
        => value => builder => builder.AddContent(0, value);

    public static RenderFragment<T> Decorate<T>(string tag, string cssClass, RenderFragment<T> inner)
        => value => builder =>
        {
            builder.OpenElement(0, tag);
            builder.AddAttribute(1, "class", cssClass);
            builder.AddContent(2, inner(value));
            builder.CloseElement();
        };

    // The map happens on the value, before the inner template ever sees it.
    public static RenderFragment<TIn> Adapt<TIn, TOut>(Func<TIn, TOut> select, RenderFragment<TOut> inner)
        => value => inner(select(value));

    public static RenderFragment ForEach<T>(IEnumerable<T> items, RenderFragment<T> template)
        => builder =>
        {
            foreach (var item in items)
            {
                template(item)(builder);
            }
        };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var row = Decorate<Person>("li", "person", Adapt<Person, string>(person => person.Name, Label()));

        builder.OpenElement(0, "ul");
        builder.AddAttribute(1, "class", "people");
        builder.AddContent(2, ForEach(People, row));
        builder.CloseElement();
    }
}
