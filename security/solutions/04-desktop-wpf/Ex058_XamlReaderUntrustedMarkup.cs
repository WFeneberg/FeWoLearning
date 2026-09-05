using System.IO;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Xml;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 058 — XamlReaderUntrustedMarkup (reference solution).
public static class Ex058_XamlReaderUntrustedMarkup
{
    private const string PresentationNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
    private const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    private static readonly HashSet<string> AllowedShapeElements = new(StringComparer.Ordinal)
    {
        "Rectangle", "Ellipse", "Line", "Polygon", "Polyline", "Path",
    };

    public static bool TryLoadShape(string markup, out Shape? shape)
    {
        shape = null;
        ArgumentNullException.ThrowIfNull(markup);

        if (!IsSafeToParse(markup))
            return false;

        object root;
        try
        {
            root = XamlReader.Parse(markup);
        }
        catch
        {
            // Defense in depth: even markup that passed the scan above but the XAML
            // parser itself rejects (malformed, unsupported construct) is a refusal,
            // not a crash.
            return false;
        }

        if (root is not Shape parsedShape)
            return false;

        shape = parsedShape;
        return true;
    }

    // Walks the raw markup as plain XML - never touching XamlReader - and refuses
    // anything that is not a lone, known Shape element in the standard presentation
    // namespace. This runs BEFORE XamlReader ever sees the string, so a disallowed
    // construct (ObjectDataProvider, x:Code, a clr-namespace mapping) is never even
    // handed to the markup parser, let alone instantiated.
    private static bool IsSafeToParse(string markup)
    {
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit };
        using var stringReader = new StringReader(markup);
        using var xmlReader = XmlReader.Create(stringReader, settings);

        var sawRoot = false;
        try
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                var ns = xmlReader.NamespaceURI;
                var name = xmlReader.LocalName;

                if (ns == XamlNamespace && name == "Code")
                    return false; // x:Code: compiled code-behind, never allowed at runtime here.

                if (!sawRoot)
                {
                    sawRoot = true;
                    if (ns != PresentationNamespace || !AllowedShapeElements.Contains(name))
                        return false; // root must be a known Shape element in the standard namespace.
                }
                else if (ns != PresentationNamespace && ns != XamlNamespace)
                {
                    return false; // nested elements must stay in the same trusted namespaces.
                }

                if (!xmlReader.HasAttributes)
                    continue;

                for (var i = 0; i < xmlReader.AttributeCount; i++)
                {
                    xmlReader.MoveToAttribute(i);
                    if (xmlReader.Value.Contains("clr-namespace:", StringComparison.OrdinalIgnoreCase))
                        return false; // any clr-namespace mapping opens the door to arbitrary CLR types.
                }

                xmlReader.MoveToElement();
            }
        }
        catch (XmlException)
        {
            return false;
        }

        return sawRoot;
    }
}
