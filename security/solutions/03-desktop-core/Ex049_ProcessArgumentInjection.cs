using System.Diagnostics;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 049 — ProcessArgumentInjection (reference solution).
public static class Ex049_ProcessArgumentInjection
{
    public static ProcessStartInfo BuildStartInfo(string executable, IReadOnlyList<string> arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,

            // UseShellExecute = false is required for ArgumentList to be honoured
            // at all, and it also means CreateProcess builds the argument vector
            // directly - no cmd.exe re-parsing a joined string, so there is no
            // shell metacharacter for an argument to inject through.
            UseShellExecute = false,
        };

        // Each argument becomes its own already-delimited entry in the process's
        // argument vector; nothing here ever concatenates arguments into a single
        // string that would need manual, error-prone quoting/escaping.
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        return startInfo;
    }
}
