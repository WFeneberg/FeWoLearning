using System.IO;
using System.Text;
using System.Xml;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex046_XmlExternalEntityTests : IDisposable
{
    private readonly string _directory =
        Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
    }

    private static Stream ToStream(string xml) => new MemoryStream(Encoding.UTF8.GetBytes(xml));

    [Fact]
    public void Attack_External_Entity_Pointing_At_A_Local_File_Never_Leaks_Its_Contents()
    {
        Directory.CreateDirectory(_directory);
        var sentinel = "SENTINEL_" + Guid.NewGuid().ToString("N");
        var sentinelFile = Path.Combine(_directory, "secret.txt");
        File.WriteAllText(sentinelFile, sentinel);
        var fileUri = new Uri(sentinelFile).AbsoluteUri;

        var xml = $"""
            <?xml version="1.0"?>
            <!DOCTYPE root [
              <!ENTITY xxe SYSTEM "{fileUri}">
            ]>
            <root><title>&xxe;</title></root>
            """;

        string? title = null;
        try
        {
            title = Ex046_XmlExternalEntity.ReadTitle(ToStream(xml));
        }
        catch (XmlException)
        {
            // Rejecting the document outright - because it declares a
            // DOCTYPE at all - is an acceptable defense too.
            return;
        }

        Assert.DoesNotContain(sentinel, title ?? string.Empty);
    }

    [Fact]
    public void Attack_Billion_Laughs_Fails_Fast_Instead_Of_Hanging_Or_Exhausting_Memory()
    {
        const string xml = """
            <?xml version="1.0"?>
            <!DOCTYPE lolz [
              <!ENTITY a "lol">
              <!ENTITY b "&a;&a;&a;&a;&a;&a;&a;&a;&a;&a;">
              <!ENTITY c "&b;&b;&b;&b;&b;&b;&b;&b;&b;&b;">
              <!ENTITY d "&c;&c;&c;&c;&c;&c;&c;&c;&c;&c;">
              <!ENTITY e "&d;&d;&d;&d;&d;&d;&d;&d;&d;&d;">
            ]>
            <root><title>&e;</title></root>
            """;

        // DTD processing is refused as soon as the parser sees the DOCTYPE
        // token, before a single entity is ever expanded - so this throws
        // immediately rather than expanding anything. No timeout needed: a
        // correct implementation cannot reach the expansion step at all.
        Assert.Throws<XmlException>(() => Ex046_XmlExternalEntity.ReadTitle(ToStream(xml)));
    }

    [Fact]
    public void Attack_External_Dtd_Reference_Never_Attempts_The_Fetch()
    {
        // ".invalid" is reserved (RFC 2606) and never resolves, so even a
        // wrong implementation that tried to fetch this would fail fast
        // rather than hang the test run.
        const string xml = """
            <?xml version="1.0"?>
            <!DOCTYPE root SYSTEM "http://xxe-should-never-be-fetched.invalid/external.dtd">
            <root><title>Hello</title></root>
            """;

        Assert.Throws<XmlException>(() => Ex046_XmlExternalEntity.ReadTitle(ToStream(xml)));
    }

    [Fact]
    public void Use_A_Plain_Well_Formed_Document_Returns_Its_Title()
    {
        const string xml = "<root><title>Hello, World!</title></root>";

        Assert.Equal("Hello, World!", Ex046_XmlExternalEntity.ReadTitle(ToStream(xml)));
    }

    [Fact]
    public void Use_A_Document_With_A_Utf8_Bom_And_A_Namespace_Still_Returns_Its_Title()
    {
        const string xml = """<root xmlns="urn:fewo:test"><title>Namespaced Title</title></root>""";
        var bytes = new byte[] { 0xEF, 0xBB, 0xBF }.Concat(Encoding.UTF8.GetBytes(xml)).ToArray();

        Assert.Equal("Namespaced Title", Ex046_XmlExternalEntity.ReadTitle(new MemoryStream(bytes)));
    }
}
