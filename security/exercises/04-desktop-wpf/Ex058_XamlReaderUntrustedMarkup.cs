using System.Windows.Shapes;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 058 — XamlReaderUntrustedMarkup (desktop-wpf).
// Goal:   XamlReader.Parse/Load is a code-execution surface, not a data-parsing one:
//         XAML can instantiate ANY type reachable through a clr-namespace mapping
//         (including ObjectDataProvider, which constructs or invokes a method on an
//         arbitrary CLR type) and can even declare a code-behind block via x:Code.
//         Handing untrusted markup — a saved layout, a pasted snippet, anything that
//         did not ship with the app — straight to XamlReader is equivalent to
//         deserializing untrusted data with a type-name-driven binder. This exercise
//         is a narrow, safe loader: it must decide whether markup is *inert enough to
//         parse at all* before ever calling into XamlReader, not merely check the
//         type of whatever XamlReader happens to hand back.
// Drills: XamlReader.Parse as code execution, restricting parsed markup.
// Passes: attack facts   - markup whose root is not one of a small set of known Shape
//                          elements (an ObjectDataProvider, a Window) is refused; markup
//                          declaring an `x:Code` block anywhere is refused; markup that
//                          declares a `clr-namespace:` mapping anywhere (even on the
//                          root element itself, even naming something as ordinary as
//                          System.Object) is refused. All three must be rejected before
//                          XamlReader.Parse is ever called on them — never by letting
//                          the constructed object run and hoping it turns out harmless.
//         use facts      - a plain `<Rectangle Width="10" Height="4"/>` loads, and the
//                          returned Rectangle's Width is 10; a plain `<Ellipse/>` also
//                          loads. Both use the standard presentation namespace only.
public static class Ex058_XamlReaderUntrustedMarkup
{
    public static bool TryLoadShape(string markup, out Shape? shape) =>
        throw new NotImplementedException(
            "TODO: Ex058 - scan the raw markup first: refuse anything whose root is not an allowlisted " +
            "Shape element in the standard presentation namespace, anything declaring x:Code, and anything " +
            "declaring a clr-namespace: mapping; only call XamlReader.Parse once the scan passes");
}
