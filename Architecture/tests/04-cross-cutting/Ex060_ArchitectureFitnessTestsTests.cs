using FeWoLearning.Architecture.Exercises.CrossCutting;
using FeWoLearning.Architecture.Exercises.CrossCutting.Ex060.Domain;
using Microsoft.Data.Sqlite;

namespace FeWoLearning.Architecture.Tests.CrossCutting;

public class Ex060_ArchitectureFitnessTestsTests
{
    private const string DomainSuffix = ".Ex060.Domain";
    private const string Rule = "the domain does not touch infrastructure";

    private static IReadOnlyList<DependencyViolation> Run(Func<Type, bool> forbidden) =>
        Ex060_ArchitectureFitnessTests.Check(
            typeof(PricingPolicy).Assembly, DomainSuffix, forbidden, Rule);

    /// <summary>Forbids this assembly's own infrastructure namespace AND the SQLite package.</summary>
    private static bool IsInfrastructure(Type type) =>
        type.Namespace?.EndsWith(".Ex060.Infrastructure", StringComparison.Ordinal) == true
        || type.Assembly.GetName().Name == "Microsoft.Data.Sqlite";

    [Fact]
    public void A_Clean_Type_Is_Not_Reported()
    {
        // Paired with the two below - alone, a rule that returns nothing satisfies it,
        // which is exactly the fitness rule that has never caught anything and has
        // therefore never been shown to work.
        Assert.DoesNotContain(Run(IsInfrastructure), v => v.FromType == nameof(PricingPolicy));
    }

    [Fact]
    public void A_Leak_Inside_This_Assembly_Is_Reported()
    {
        Assert.Contains(Run(IsInfrastructure), v => v.FromType == nameof(LeaksToLocalInfrastructure));
    }

    [Fact]
    public void Mechanism_A_Leak_Into_A_Referenced_Assembly_Is_Reported_Too()
    {
        // The fact that separates a real fitness rule from one that only knows about
        // namespaces in its own project. It is also the commonest leak there is: taking a
        // NuGet dependency does not feel like crossing a layer, so nobody notices that
        // the domain now needs a database driver to compile.
        var violation = Assert.Single(
            Run(IsInfrastructure), v => v.FromType == nameof(LeaksToAReferencedAssembly));

        Assert.Equal(nameof(SqliteConnection), violation.ToType);
    }

    [Fact]
    public void Mechanism_A_Violation_Names_The_Rule_And_Both_Ends()
    {
        // "Layering violated" is not a finding, it is a mood: whoever reads it still has
        // to go and find the offender, and a rule that is annoying to act on gets
        // suppressed. Naming the rule matters as much - a failure nobody can trace back
        // to a decision looks like the tooling being difficult.
        var violation = Assert.Single(
            Run(IsInfrastructure), v => v.FromType == nameof(LeaksToLocalInfrastructure));

        Assert.Equal(Rule, violation.Rule);
        Assert.Equal("SqlStore", violation.ToType);
    }

    [Fact]
    public void A_Rule_With_Nothing_To_Find_Reports_Nothing()
    {
        // The other half of "detect rather than merely fail to find": a rule that reports
        // everything is as useless as one that reports nothing, and is much louder.
        Assert.Empty(Run(_ => false));
    }

    [Fact]
    public void A_Namespace_With_No_Types_Reports_Nothing_Rather_Than_Throwing()
    {
        Assert.Empty(Ex060_ArchitectureFitnessTests.Check(
            typeof(PricingPolicy).Assembly, ".NoSuchNamespace", IsInfrastructure, Rule));
    }
}
