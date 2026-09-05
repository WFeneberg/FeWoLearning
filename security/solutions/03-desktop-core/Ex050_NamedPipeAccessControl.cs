using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 050 — NamedPipeAccessControl (reference solution).
public static class Ex050_NamedPipeAccessControl
{
    public static NamedPipeServerStream CreateServer(string pipeName)
    {
        var currentUser = WindowsIdentity.GetCurrent().User
            ?? throw new InvalidOperationException("Unable to resolve the current user's SID.");

        var pipeSecurity = new PipeSecurity();

        // The only explicit rule on this pipe: the current user may read and
        // write. Nothing grants any right - including ChangePermissions - to
        // "Everyone", "Authenticated Users", or any other identity, so a peer
        // running as a different account gets nothing at all.
        pipeSecurity.AddAccessRule(new PipeAccessRule(
            currentUser,
            PipeAccessRights.ReadWrite,
            AccessControlType.Allow));

        return NamedPipeServerStreamAcl.Create(
            pipeName,
            PipeDirection.InOut,
            maxNumberOfServerInstances: 1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous,
            inBufferSize: 0,
            outBufferSize: 0,
            pipeSecurity,
            HandleInheritability.None,
            additionalAccessRights: (PipeAccessRights)0);
    }
}
