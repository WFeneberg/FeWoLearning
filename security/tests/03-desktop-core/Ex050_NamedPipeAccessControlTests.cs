using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex050_NamedPipeAccessControlTests
{
    private static string NewPipeName() => $"fewo-sec-{Guid.NewGuid():N}";

    [Fact]
    public void Attack_The_Pipe_Grants_No_Rights_To_The_World_Sid()
    {
        using var server = Ex050_NamedPipeAccessControl.CreateServer(NewPipeName());

        var pipeSecurity = server.GetAccessControl();
        var world = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

        foreach (PipeAccessRule rule in pipeSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier)))
        {
            if (rule.AccessControlType != AccessControlType.Allow) continue;

            Assert.False(
                ((SecurityIdentifier)rule.IdentityReference).Equals(world),
                "the World SID must not be granted any rights on the pipe");
        }
    }

    [Fact]
    public void Attack_The_Pipe_Grants_No_Rights_To_Authenticated_Users()
    {
        using var server = Ex050_NamedPipeAccessControl.CreateServer(NewPipeName());

        var pipeSecurity = server.GetAccessControl();
        var authenticatedUsers = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

        foreach (PipeAccessRule rule in pipeSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier)))
        {
            if (rule.AccessControlType != AccessControlType.Allow) continue;

            Assert.False(
                ((SecurityIdentifier)rule.IdentityReference).Equals(authenticatedUsers),
                "AuthenticatedUsers must not be granted any rights on the pipe");
        }
    }

    [Fact]
    public void Attack_ChangePermissions_Is_Not_Granted_To_Anyone_But_The_Owner()
    {
        using var server = Ex050_NamedPipeAccessControl.CreateServer(NewPipeName());

        var pipeSecurity = server.GetAccessControl();
        var currentUser = WindowsIdentity.GetCurrent().User!;

        foreach (PipeAccessRule rule in pipeSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier)))
        {
            if (rule.AccessControlType != AccessControlType.Allow) continue;
            if (((SecurityIdentifier)rule.IdentityReference).Equals(currentUser)) continue;

            Assert.False(
                rule.PipeAccessRights.HasFlag(PipeAccessRights.ChangePermissions),
                "only the owner may be granted ChangePermissions on the pipe");
        }
    }

    [Fact]
    public void Use_The_Current_User_Has_ReadWrite()
    {
        using var server = Ex050_NamedPipeAccessControl.CreateServer(NewPipeName());

        var pipeSecurity = server.GetAccessControl();
        var currentUser = WindowsIdentity.GetCurrent().User!;

        var grantsReadWrite = false;
        foreach (PipeAccessRule rule in pipeSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier)))
        {
            if (rule.AccessControlType != AccessControlType.Allow) continue;
            if (!((SecurityIdentifier)rule.IdentityReference).Equals(currentUser)) continue;

            if ((rule.PipeAccessRights & PipeAccessRights.ReadWrite) == PipeAccessRights.ReadWrite)
            {
                grantsReadWrite = true;
            }
        }

        Assert.True(grantsReadWrite, "the current user must be granted ReadWrite on the pipe");
    }

    [Fact]
    public async Task Use_A_Client_Connecting_As_The_Current_User_Completes_A_Round_Trip()
    {
        var pipeName = NewPipeName();
        using var server = Ex050_NamedPipeAccessControl.CreateServer(pipeName);

        // A hard local timeout, in addition to the test's own cancellation token:
        // this round-trip must never be able to hang the whole (serialised) suite,
        // no matter which side of the exchange fails to show up.
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            timeoutCts.Token, TestContext.Current.CancellationToken);
        var token = linkedCts.Token;

        // Kick off the client before awaiting the server's connection, so both
        // sides of the handshake are in flight concurrently rather than one
        // blocking on the other with nothing driving the far end yet.
        var clientTask = Task.Run(async () =>
        {
            using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            await client.ConnectAsync(token);

            await client.WriteAsync(Encoding.UTF8.GetBytes("ping"), token);

            var buffer = new byte[64];
            var read = await client.ReadAsync(buffer, token);
            return Encoding.UTF8.GetString(buffer, 0, read);
        }, token);

        await server.WaitForConnectionAsync(token);

        var serverBuffer = new byte[64];
        var serverRead = await server.ReadAsync(serverBuffer, token);
        var received = Encoding.UTF8.GetString(serverBuffer, 0, serverRead);

        await server.WriteAsync(Encoding.UTF8.GetBytes(received + "-pong"), token);

        var clientResult = await clientTask;

        Assert.Equal("ping-pong", clientResult);
    }
}
