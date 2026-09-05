using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 041 — FixedTimeComparison (desktop-core).
// Goal:   Compare a presented token against an expected one without letting the
//         comparison's structure leak how much of the token was right - hash (or
//         pad) both sides to the same fixed length first, then compare with
//         CryptographicOperations.FixedTimeEquals, never `==` or a
//         character-by-character loop that can return early.
// Drills: CryptographicOperations.FixedTimeEquals, why length-first exits leak.
// Passes: attack facts   - a presented token that is an exact *prefix* of the
//                          expected one returns false, and so does one sharing
//                          the expected token's first 31 of its 32 characters
//                          (only the last character differs);
//         use facts      - two identical tokens return true; the comparison is
//                          ordinal, so tokens differing only by case return
//                          false; an empty presented token against an empty
//                          expected token returns true.
//         Grading is entirely by these behavioural/structural facts - never by
//         elapsed time. Do not add a timing assertion to this exercise or its
//         test: measuring wall-clock time is exactly the flaky, environment-
//         dependent approach a fixed-time comparison exists to make unnecessary.
public static class Ex041_FixedTimeComparison
{
    public static bool TokensMatch(string presented, string expected) =>
        throw new NotImplementedException(
            "TODO: Ex041 - hash both strings to a fixed length and compare with CryptographicOperations.FixedTimeEquals");
}
