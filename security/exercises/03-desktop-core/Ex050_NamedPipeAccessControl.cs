using System.IO.Pipes;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 050 — NamedPipeAccessControl (desktop-core).
// Goal:   Create a named pipe server that only the current user may talk to.
//         A named pipe with no explicit security descriptor inherits a default
//         DACL that is far more permissive than most callers expect, so any
//         local process - including one running as a different, unprivileged
//         account - could otherwise connect to it.
// Drills: PipeSecurity, ACLs, rejecting unauthorised peers.
// Passes: attack facts   - the pipe's PipeSecurity grants no rights at all to
//                          WellKnownSidType.WorldSid or AuthenticatedUserSid;
//                          it does not grant PipeAccessRights.ChangePermissions
//                          to anyone but the owner (the current user);
//         use facts      - the current user has ReadWrite; and a client
//                          connecting as the current user actually completes a
//                          round-trip message over the pipe this method
//                          returns.
public static class Ex050_NamedPipeAccessControl
{
    public static NamedPipeServerStream CreateServer(string pipeName) =>
        throw new NotImplementedException(
            "TODO: Ex050 - build a PipeSecurity that grants only the current user's SID ReadWrite, and create the " +
            "server with that security descriptor instead of the default one");
}
