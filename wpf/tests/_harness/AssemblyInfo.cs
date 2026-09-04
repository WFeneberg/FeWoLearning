using Xunit.Sdk;
using Xunit.v3;

// SystemResources, the theme dictionaries and Application.Current are process-global,
// and Application can only be constructed once per process. Serial it is.
// The brief's [assembly: CollectionBehavior(DisableTestParallelization = true)] does not
// compile under xunit.v3 4.0.0: DisableTestParallelization is marked Obsolete(error: true)
// in this version, pointing at ParallelizationAttribute.Mode instead.
[assembly: Parallelization(Mode = ParallelMode.None)]
