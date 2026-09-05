using Xunit.Sdk;
using Xunit.v3;

// Rows 050 (named pipes) and 055 (the system clipboard) touch machine-global and
// process-global state, so the suite must not run in parallel.
// CollectionBehavior(DisableTestParallelization = true) is Obsolete(error: true) in
// xunit.v3 4.0.0 and does not compile - this is the replacement.
[assembly: Parallelization(Mode = ParallelMode.None)]
