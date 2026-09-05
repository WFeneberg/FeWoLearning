namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 052 — RestrictiveFileAcl (desktop-core).
// Goal:   Write a secret to disk with an ACL locked down at creation time -
//         nothing but the current user, and no inherited entries from the
//         parent directory. Fixing permissions *after* creation leaves a window
//         where the file briefly carries whatever the parent handed it, and
//         "inherited but harmless" is not something you can prove after the
//         fact - the file has to be created already protected.
// Drills: file ACLs at creation, inherited permissions, least privilege.
// Passes: attack facts   - the created file's ACL grants nothing to
//                          WellKnownSidType.WorldSid or AuthenticatedUserSid;
//                          ACL inheritance is disabled on the file itself, so a
//                          permissive parent directory cannot leak rights onto
//                          it;
//         use facts      - the current user can read the file back and its
//                          content matches what was written; and writing to the
//                          same path a second time replaces the content rather
//                          than appending to it.
public static class Ex052_RestrictiveFileAcl
{
    public static void WriteSecret(string path, byte[] content) =>
        throw new NotImplementedException(
            "TODO: Ex052 - create/overwrite the file with a FileSecurity that is protected from inheritance and " +
            "grants only the current user's SID access, applied atomically at creation, not fixed up afterwards");
}
