using System.IO;
using System.Xml;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 046 — XmlExternalEntity (reference solution).
public static class Ex046_XmlExternalEntity
{
    public static string? ReadTitle(Stream xml)
    {
        var settings = new XmlReaderSettings
        {
            // Refuses any DOCTYPE outright - internal subset, external
            // reference, or nested entity bomb alike - before any of it is
            // ever parsed or expanded.
            DtdProcessing = DtdProcessing.Prohibit,
            // Belt and suspenders: even if DtdProcessing were ever relaxed to
            // Parse for some legitimate reason, a null resolver still refuses
            // to fetch anything an entity or external subset points at.
            XmlResolver = null,
        };

        using var reader = XmlReader.Create(xml, settings);
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "title")
            {
                return reader.ReadElementContentAsString();
            }
        }

        return null;
    }
}
