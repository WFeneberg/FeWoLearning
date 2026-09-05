using System.Diagnostics;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 049 — ProcessArgumentInjection (desktop-core).
// Goal:   Build a ProcessStartInfo from a caller-supplied argument list without
//         ever assembling a single "Arguments" string - that string is
//         re-parsed by the shell/CRT, so a value containing a quote or an
//         ampersand can inject an extra command; ArgumentList hands each
//         argument to CreateProcess as its own already-delimited entry.
// Drills: ProcessStartInfo.ArgumentList over a joined Arguments string.
// Passes: attack facts   - an argument containing `" & del /q *` still appears
//                          as exactly one ArgumentList entry, and the
//                          `Arguments` string property is empty; UseShellExecute
//                          is false (required for ArgumentList to be honoured at
//                          all, and it also turns off shell metacharacter
//                          handling); an argument containing an embedded newline
//                          stays one entry, not several;
//         use facts      - three ordinary arguments appear as three
//                          ArgumentList entries, in order, verbatim; and
//                          FileName equals the executable passed in. This
//                          method never starts the process - it only builds and
//                          returns the ProcessStartInfo for inspection.
public static class Ex049_ProcessArgumentInjection
{
    public static ProcessStartInfo BuildStartInfo(string executable, IReadOnlyList<string> arguments) =>
        throw new NotImplementedException(
            "TODO: Ex049 - build a ProcessStartInfo with FileName = executable, UseShellExecute = false, and each " +
            "argument added to ArgumentList (never concatenated into the Arguments string)");
}
