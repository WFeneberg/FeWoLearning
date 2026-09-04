// Uno's runtime is single-threaded by construction and the harness declares the test
// thread to be the UI thread, so two collections running at once would share one
// dispatcher and one Application. Keep the suite serial.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
