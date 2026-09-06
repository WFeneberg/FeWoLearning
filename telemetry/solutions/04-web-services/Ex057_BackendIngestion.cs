using System.Text.Json;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 057 — BackendIngestion (web-services). 🐳
// Goal:   Watch a structured log arrive somewhere that can be asked questions, and see
//         exactly which part makes the questions possible.
// Drills: the CLEF wire format, message templates on the wire, property types.
// Passes: a rendered line carries @t and @mt, and @mt is the TEMPLATE with its
//                     placeholders intact;
//         each property is its own JSON member under its own name;
//         a numeric property is a JSON NUMBER, not a quoted string;
//         a property named like a CLEF control field is escaped rather than colliding
//                     with it;
//         and 🐳 a real Seq accepts a batch and answers a query that filters on one of
//                     those fields.
//
// Row 001 said a message template keeps its fields queryable and left "queryable" as an
// assertion. This is where it stops being one. The 🐳 fact asks a real backend
// "OrderId = 'O-42'" and gets the event back - which is only possible because OrderId
// arrived as a field. Interpolate at the call site and the same query returns nothing:
// not an error, not a warning, an empty result that looks exactly like "it did not
// happen".
//
// The third clause is the same lesson as row 040's integer status code, on the wire this
// time. "19" as a string cannot be compared, summed or charted, and a backend will not
// tell you why your threshold never fires.
//
// The fourth is the format's own sharp edge: CLEF reserves the names beginning with @,
// so a property genuinely called "@type" has to be written "@@type" or it is read as a
// control field and your data becomes metadata.
public static class Ex057_BackendIngestion
{
    /// <summary>Where a CLEF batch is posted.</summary>
    public const string IngestPath = "/ingest/clef";

    /// <summary>The CLEF field carrying the timestamp.</summary>
    public const string TimestampField = "@t";

    /// <summary>The CLEF field carrying the message TEMPLATE.</summary>
    public const string MessageTemplateField = "@mt";

    /// <summary>
    /// Render one event as a single CLEF line.
    ///
    /// <paramref name="messageTemplate"/> goes into <see cref="MessageTemplateField"/>
    /// verbatim - placeholders and all, unrendered. <paramref name="at"/> goes into
    /// <see cref="TimestampField"/> in round-trip ("O") form.
    ///
    /// Each entry of <paramref name="properties"/> becomes its own JSON member, keeping
    /// its type: a number stays a number, a boolean stays a boolean. A name that starts
    /// with '@' is escaped by doubling it, so it is read as data rather than as a control
    /// field.
    /// </summary>
    public static string ToClefLine(
        DateTimeOffset at, string messageTemplate, IReadOnlyDictionary<string, object?> properties)
    {
        var document = new Dictionary<string, object?>
        {
            [TimestampField] = at.ToString("O"),

            // Verbatim. Rendering it here would destroy both halves at once: the template
            // stops being constant and the fields stop existing.
            [MessageTemplateField] = messageTemplate,
        };

        foreach (var (name, value) in properties)
        {
            // A name beginning with @ is escaped by doubling, or the backend reads it as
            // a control field and the data silently becomes metadata.
            var key = name.StartsWith('@') ? "@" + name : name;

            document[key] = value;
        }

        // Serialized as object?, so a number stays a JSON number and a boolean a boolean.
        // Stringifying here is what makes "Amount > 100" impossible downstream.
        return JsonSerializer.Serialize(document);
    }

    /// <summary>
    /// Join CLEF lines into a batch. The format is newline-delimited JSON: one complete
    /// object per line, and no enclosing array.
    /// </summary>
    public static string ToClefBatch(IEnumerable<string> lines) =>
        // Newline-delimited JSON: one complete object per line, no enclosing array. That
        // is what lets a backend accept a partial batch and a writer append without
        // rewriting anything.
        string.Join("\n", lines);
}
