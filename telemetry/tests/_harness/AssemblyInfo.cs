// The whole suite runs serially, and this is not optional.
//
// Every ActivityListener sees activities from EVERY test running concurrently, and
// Sdk.SetDefaultTextMapPropagator, Activity.DefaultIdFormat and Activity.Current are
// process-wide. A parallel run does not error - it silently produces cross-test
// contamination, and (per avalonia/'s 2026-09-05 finding) can truncate the run while
// still printing a normal-looking summary. Read the test COUNT, not just the word
// "Failed".
[assembly: CollectionBehavior(DisableTestParallelization = true)]
