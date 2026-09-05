using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 052 — RestrictiveFileAcl (reference solution).
public static class Ex052_RestrictiveFileAcl
{
    public static void WriteSecret(string path, byte[] content)
    {
        var currentUser = WindowsIdentity.GetCurrent().User
            ?? throw new InvalidOperationException("Unable to resolve the current user's SID.");

        var security = new FileSecurity();

        // Protect this DACL from the parent directory's inheritable entries and
        // drop any it would otherwise pick up - a permissive parent must not be
        // able to leak rights onto this file.
        security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
        security.AddAccessRule(new FileSystemAccessRule(
            currentUser,
            FileSystemRights.FullControl,
            AccessControlType.Allow));

        // FileSystemAclExtensions.Create() applies this security descriptor as
        // part of the underlying CreateFile call itself, so the file never
        // exists - even for an instant - with the default, more permissive ACL a
        // plain File.Create()/FileStream would apply before being fixed up
        // afterwards. FileMode.Create truncates an existing file, which is also
        // what makes a second WriteSecret to the same path replace rather than
        // append.
        using var stream = new FileInfo(path).Create(
            FileMode.Create,
            FileSystemRights.Write,
            FileShare.None,
            bufferSize: 4096,
            FileOptions.None,
            security);

        stream.Write(content, 0, content.Length);
    }
}
