using Bunit;

namespace FeWoLearning.Security.Tests.Harness;

// Thin wrapper over BunitContext so block-02 tests have one place to add the
// services the block needs (auth state, navigation, persistent component state).
//
// Note for every test file in this project: bUnit 2.9 still ships an obsolete
// Bunit.TestContext, which collides with xunit.v3's Xunit.TestContext. Any file
// that has `using Bunit;` and also touches TestContext fails CS0104. Add
// `using TestContext = Xunit.TestContext;` to those files.
public sealed class BlazorHarness : BunitContext
{
}
