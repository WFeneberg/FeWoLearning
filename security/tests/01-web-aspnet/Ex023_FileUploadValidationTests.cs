using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex023_FileUploadValidationTests
{
    private static readonly byte[] PngMagicBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private static byte[] GenuinePng(int extraBytes = 16) => [.. PngMagicBytes, .. new byte[extraBytes]];

    [Fact]
    public void Attack_An_Exe_Extension_Is_Rejected_Outright()
    {
        var accepted = Ex023_FileUploadValidation.TryAccept(
            "payload.exe", GenuinePng(), maxBytes: 1_000_000, out var storageName, out var rejection);

        Assert.False(accepted);
        Assert.NotNull(rejection);
        Assert.Equal("", storageName);
    }

    [Fact]
    public void Attack_An_Executable_Disguised_With_A_Pdf_Extension_Is_Rejected()
    {
        byte[] mzHeader = [0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00];

        var accepted = Ex023_FileUploadValidation.TryAccept(
            "report.pdf", mzHeader, maxBytes: 1_000_000, out var storageName, out var rejection);

        Assert.False(accepted);
        Assert.NotNull(rejection);
        Assert.Equal("", storageName);
    }

    [Fact]
    public void Attack_A_File_Over_The_Size_Limit_Is_Rejected()
    {
        var content = GenuinePng(extraBytes: 100);

        var accepted = Ex023_FileUploadValidation.TryAccept(
            "photo.png", content, maxBytes: content.Length - 1, out var storageName, out var rejection);

        Assert.False(accepted);
        Assert.NotNull(rejection);
        Assert.Equal("", storageName);
    }

    [Fact]
    public void Attack_A_Path_Traversal_Filename_Never_Reaches_The_Storage_Name()
    {
        var accepted = Ex023_FileUploadValidation.TryAccept(
            "../../evil.png", GenuinePng(), maxBytes: 1_000_000, out var storageName, out _);

        Assert.True(accepted);
        Assert.DoesNotContain("..", storageName);
        Assert.DoesNotContain("/", storageName);
        Assert.DoesNotContain("\\", storageName);
    }

    [Fact]
    public void Use_A_Genuine_Png_Is_Accepted_With_An_Unpredictable_Storage_Name()
    {
        var accepted = Ex023_FileUploadValidation.TryAccept(
            "photo.png", GenuinePng(), maxBytes: 1_000_000, out var storageName, out var rejection);

        Assert.True(accepted);
        Assert.Null(rejection);
        Assert.EndsWith(".png", storageName);
        Assert.NotEqual("photo.png", storageName);
    }

    [Fact]
    public void Use_Two_Uploads_Of_The_Same_Name_Produce_Different_Storage_Names()
    {
        Ex023_FileUploadValidation.TryAccept("photo.png", GenuinePng(), maxBytes: 1_000_000, out var first, out _);
        Ex023_FileUploadValidation.TryAccept("photo.png", GenuinePng(), maxBytes: 1_000_000, out var second, out _);

        Assert.NotEqual(first, second);
    }
}
