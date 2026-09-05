using System.IO;
using System.Text;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex038_CredentialStorageTests : IDisposable
{
    private readonly string _directory =
        Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
    }

    [Fact]
    public void Attack_No_File_Under_The_Directory_Contains_The_Secret_In_Plaintext()
    {
        var store = new Ex038_CredentialStore(_directory);
        var secret = "a-distinctive-secret-marker-4b2e91";
        store.Save("api-key", secret);

        var files = Directory.GetFiles(_directory, "*", SearchOption.AllDirectories);
        Assert.NotEmpty(files); // Save must actually have written something.

        var needle = Encoding.UTF8.GetBytes(secret);
        foreach (var file in files)
        {
            var bytes = File.ReadAllBytes(file);
            Assert.False(ContainsSubsequence(bytes, needle), $"{file} contains the secret in plaintext");
        }
    }

    [Fact]
    public void Attack_Load_For_An_Unknown_Name_Returns_Null()
    {
        var store = new Ex038_CredentialStore(_directory);

        Assert.Null(store.Load("no-such-credential"));
    }

    [Fact]
    public void Use_Load_Returns_Exactly_What_Save_Stored_For_Unicode()
    {
        var store = new Ex038_CredentialStore(_directory);
        var secret = "héllo wörld — 漢字 — пароль";

        store.Save("unicode-secret", secret);

        Assert.Equal(secret, store.Load("unicode-secret"));
    }

    [Fact]
    public void Use_Load_Returns_Exactly_What_Save_Stored_For_A_4KB_Secret()
    {
        var store = new Ex038_CredentialStore(_directory);
        var secret = new string('x', 4096);

        store.Save("large-secret", secret);

        Assert.Equal(secret, store.Load("large-secret"));
    }

    [Fact]
    public void Use_Saving_Twice_Under_One_Name_Overwrites_Rather_Than_Appends()
    {
        var store = new Ex038_CredentialStore(_directory);

        store.Save("rotating", "first-value");
        store.Save("rotating", "second-value");

        Assert.Equal("second-value", store.Load("rotating"));
    }

    private static bool ContainsSubsequence(byte[] haystack, byte[] needle)
    {
        if (needle.Length == 0) return true;

        for (var i = 0; i <= haystack.Length - needle.Length; i++)
        {
            var match = true;
            for (var j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j] != needle[j])
                {
                    match = false;
                    break;
                }
            }

            if (match) return true;
        }

        return false;
    }
}
