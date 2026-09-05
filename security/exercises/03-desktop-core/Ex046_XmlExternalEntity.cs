using System.IO;
using System.Xml;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 046 — XmlExternalEntity (desktop-core).
// Goal:   Read the text of the first <title> element out of an XML document
//         (regardless of what namespace it is declared in) while refusing to
//         process any DOCTYPE at all - internal, external, or entity-bomb
//         alike. A DOCTYPE is exactly the mechanism XXE and "billion laughs"
//         both depend on, so the safe default is to reject it outright rather
//         than to try to tell a benign one from a hostile one.
// Drills: XmlReaderSettings, DtdProcessing, XmlResolver, entity expansion.
// Passes: attack facts   - a document declaring an external entity that
//                          points at a local file never returns that file's
//                          contents (a sentinel written into the file never
//                          appears in the result, whether ReadTitle rejects
//                          the document outright or returns a title that
//                          simply never resolved the entity); a "billion
//                          laughs" document (nested entity expansion) fails
//                          fast with a specific, deterministic exception
//                          rather than hanging or exhausting memory, because
//                          DTD processing is refused before any entity is
//                          ever expanded; a document whose DOCTYPE names an
//                          external DTD never attempts to fetch it;
//         use facts      - a plain well-formed document with no DOCTYPE at
//                          all returns its title; a document with a UTF-8 BOM
//                          and a default XML namespace still returns its
//                          title, because <title> is matched by local name,
//                          not by namespace-qualified name.
public static class Ex046_XmlExternalEntity
{
    public static string? ReadTitle(Stream xml) =>
        throw new NotImplementedException(
            "TODO: Ex046 - create an XmlReader with DtdProcessing = Prohibit and XmlResolver = null, then scan for the first element whose LocalName is \"title\" and return its text content");
}
