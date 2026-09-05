namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 012 — PasswordHashingPbkdf2 (web-aspnet).
// Goal:   Hash a password with PBKDF2 behind a fresh random salt per hash and a
//         high iteration count, and verify a candidate password against a
//         previously stored value - so a database leak alone yields nothing an
//         attacker can crack cheaply.
// Drills: Rfc2898DeriveBytes, per-user salt, iteration count, fixed-time verify.
// Passes: attack facts   - hashing the same password twice yields different
//                          stored values; Verify rejects a wrong password and
//                          rejects a stored value whose salt was altered by one
//                          byte; the stored value never contains the password
//                          as a substring;
//         use facts      - Verify(p, Hash(p)) is true for several passwords,
//                          including Unicode and an empty string, and the
//                          iteration count - read back out of the stored value
//                          itself - is at least 100000.
public static class Ex012_PasswordHashingPbkdf2
{
    public static string Hash(string password) =>
        throw new NotImplementedException(
            "TODO: Ex012 - PBKDF2-hash password behind a fresh random salt with >=100000 iterations, encoding iterations/salt/hash into the stored value");

    public static bool Verify(string password, string stored) =>
        throw new NotImplementedException(
            "TODO: Ex012 - parse iterations/salt/hash out of stored, re-derive from password with the same parameters, and compare in fixed time");
}
