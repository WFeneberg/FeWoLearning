using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex052_RestrictiveFileAclTests : IDisposable
{
    private readonly string _directory =
        Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));

    public Ex052_RestrictiveFileAclTests()
    {
        Directory.CreateDirectory(_directory);

        // Make the parent directory itself permissive first - Everyone gets
        // inheritable FullControl - so the file's disabled-inheritance guarantee
        // is actually exercised. Without this, a file under an already-restrictive
        // temp directory would pass the inheritance-disabled attack fact for the
        // wrong reason: there would be nothing permissive to inherit from anyway.
        var directoryInfo = new DirectoryInfo(_directory);
        var directorySecurity = directoryInfo.GetAccessControl();
        var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
        directorySecurity.AddAccessRule(new FileSystemAccessRule(
            everyone,
            FileSystemRights.FullControl,
            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
            PropagationFlags.None,
            AccessControlType.Allow));
        directoryInfo.SetAccessControl(directorySecurity);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
    }

    [Fact]
    public void Attack_The_Files_Acl_Grants_Nothing_To_The_World_Sid()
    {
        var path = Path.Combine(_directory, "secret.bin");
        Ex052_RestrictiveFileAcl.WriteSecret(path, [1, 2, 3]);

        var security = new FileInfo(path).GetAccessControl();
        var world = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

        foreach (FileSystemAccessRule rule in security.GetAccessRules(true, true, typeof(SecurityIdentifier)))
        {
            if (rule.AccessControlType != AccessControlType.Allow) continue;

            Assert.False(
                ((SecurityIdentifier)rule.IdentityReference).Equals(world),
                "the World SID must not be granted any rights on the file");
        }
    }

    [Fact]
    public void Attack_The_Files_Acl_Grants_Nothing_To_Authenticated_Users()
    {
        var path = Path.Combine(_directory, "secret.bin");
        Ex052_RestrictiveFileAcl.WriteSecret(path, [1, 2, 3]);

        var security = new FileInfo(path).GetAccessControl();
        var authenticatedUsers = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

        foreach (FileSystemAccessRule rule in security.GetAccessRules(true, true, typeof(SecurityIdentifier)))
        {
            if (rule.AccessControlType != AccessControlType.Allow) continue;

            Assert.False(
                ((SecurityIdentifier)rule.IdentityReference).Equals(authenticatedUsers),
                "AuthenticatedUsers must not be granted any rights on the file");
        }
    }

    [Fact]
    public void Attack_Acl_Inheritance_Is_Disabled_On_The_File()
    {
        var path = Path.Combine(_directory, "secret.bin");
        Ex052_RestrictiveFileAcl.WriteSecret(path, [1, 2, 3]);

        var security = new FileInfo(path).GetAccessControl();

        Assert.True(security.AreAccessRulesProtected, "the file's ACL must be protected from inheritance");

        foreach (FileSystemAccessRule rule in security.GetAccessRules(true, true, typeof(SecurityIdentifier)))
        {
            Assert.False(rule.IsInherited, "no rule on the file may be inherited from the (permissive) parent directory");
        }
    }

    [Fact]
    public void Use_The_Current_User_Can_Read_The_File_Back_With_Matching_Content()
    {
        var path = Path.Combine(_directory, "secret.bin");
        byte[] content = [9, 8, 7, 6, 5];

        Ex052_RestrictiveFileAcl.WriteSecret(path, content);

        Assert.Equal(content, File.ReadAllBytes(path));
    }

    [Fact]
    public void Use_Writing_Twice_Replaces_Rather_Than_Appends()
    {
        var path = Path.Combine(_directory, "secret.bin");

        Ex052_RestrictiveFileAcl.WriteSecret(path, [1, 2, 3, 4, 5]);
        Ex052_RestrictiveFileAcl.WriteSecret(path, [9, 9]);

        Assert.Equal(new byte[] { 9, 9 }, File.ReadAllBytes(path));
    }
}
