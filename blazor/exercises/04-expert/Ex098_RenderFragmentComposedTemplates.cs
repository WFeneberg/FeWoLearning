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

    // TODO: a template that renders value as escaped text. The T-shaped counterpart
    // of ex097's Text.
    public static RenderFragment<string> Label()
        => throw new NotImplementedException("TODO: Ex098 - a text template");

    // TODO: wrap whatever inner renders in <tag class="cssClass"> … </tag>, for the
    // same T. The value is not touched - only what surrounds it.
    public static RenderFragment<T> Decorate<T>(string tag, string cssClass, RenderFragment<T> inner)
        => throw new NotImplementedException("TODO: Ex098 - decorate a template");

    // TODO: turn a template over TOut into one over TIn by mapping the value first.
    // This is what lets a string template render a Person without either of them
    // knowing about the other.
    public static RenderFragment<TIn> Adapt<TIn, TOut>(Func<TIn, TOut> select, RenderFragment<TOut> inner)
        => throw new NotImplementedException("TODO: Ex098 - adapt a template to another type");

    // TODO: apply template to every item, in order, as one fragment. An empty
    // sequence renders nothing.
    public static RenderFragment ForEach<T>(IEnumerable<T> items, RenderFragment<T> template)
        => throw new NotImplementedException("TODO: Ex098 - a fragment from a template and a sequence");

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var row = Decorate<Person>("li", "person", Adapt<Person, string>(person => person.Name, Label()));

        builder.OpenElement(0, "ul");
        builder.AddAttribute(1, "class", "people");
        builder.AddContent(2, ForEach(People, row));
        builder.CloseElement();
    }
}
