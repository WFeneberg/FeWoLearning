// Caliburn's configuration is process-global (IoC, PlatformProvider, AssemblySource,
// ViewLocator) and every WPF test owns a Dispatcher. Parallel tests trample each other.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
