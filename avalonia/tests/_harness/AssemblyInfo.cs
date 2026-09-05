// Every [AvaloniaFact] runs on the one headless dispatcher that
// AvaloniaTestApplication sets up, and there is a single Application for the whole
// assembly. xunit.v3 runs test collections in parallel by default, so two classes
// starting at once contend for that dispatcher and the run deadlocks.
//
// The failure mode is nasty enough to be worth spelling out: the run does not
// error, it simply stops. Whatever had already finished is still reported with a
// normal-looking summary line, so a truncated run reads as a completed one - only
// the test count and a missing exit code give it away. Measured on this machine
// before this attribute existed: the beginner tier passed as two halves of 52 and
// 59 tests, and hung after 4 when asked for all 111 at once, as did a plain
// `dotnet test`.
//
// Same stance, and the same one-line fix, as uno/ and caliburn/, which sit on the
// same xunit.v3 3.2.2 generation. wpf/ needs the other spelling
// ([assembly: Parallelization(Mode = ParallelMode.None)]) because
// DisableTestParallelization is Obsolete(error: true) from xunit.v3 4.0.0 on.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
