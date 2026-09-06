using Xunit;

// Every test class in this assembly runs one at a time.
//
// Not a performance knob and not caution - it is load-bearing. Several exercises grade
// PROCESS-GLOBAL state and cannot be isolated by construction:
//
//   ex022  a static ActivitySource and Meter, and an OpenTelemetry TracerProvider whose
//          ActivityListener is installed process-wide. Its negative fact asserts that an
//          UNREGISTERED source yields a null Activity - which is false the moment any
//          other test class anywhere has a provider listening to "*".
//   ex023  two static flags that select which of the three probe scenarios is being run.
//   ex025  a static, ordered hook log that each test resets before building its model.
//
// Under the default class-level parallelism those three are safe only while no other
// class happens to touch the same statics - an assembly-wide invariant that nothing
// states and nothing enforces, with 75 catalog rows still to be written against it.
// Serialising the assembly turns that invariant into a property of the test host.
//
// The spelling is version-specific. On xunit.v3 3.2.2 this attribute is the supported
// form and is NOT obsolete (verified by reflecting over xunit.v3.core 3.2.2:
// Xunit.CollectionBehaviorAttribute, AttributeTargets.Assembly, settable
// DisableTestParallelization). Do NOT copy `wpf/`'s
// [assembly: Parallelization(Mode = ParallelMode.None)] - that is the xunit.v3 4.0.0
// spelling, where THIS attribute is Obsolete(error: true). See MicroServices/README.md
// section 6.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
