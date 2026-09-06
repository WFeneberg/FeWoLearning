namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex058.Legacy
{
    /// <summary>
    /// The other context's model, in the other context's language. Nobody here gets to
    /// rename it, and nobody here should have to read it.
    /// </summary>
    public sealed class CUSTREC
    {
        public string CUST_NM { get; set; } = "";

        /// <summary>"A", "S" or "C". No, there is no enum.</summary>
        public string CUST_STAT { get; set; } = "";

        public int CRED_LIM_CENTS { get; set; }
    }
}

namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex058.Sales
{
    using Legacy;

    public enum AccountStanding
    {
        Active,
        Suspended,
        Closed,
    }

    /// <summary>Our language: a name, a standing, and money as money.</summary>
    public sealed record Customer(string Name, AccountStanding Standing, decimal CreditLimit);

    /// <summary>
    /// A deliberate violation, shipped so the fitness check has something to catch. It
    /// keeps the original around "just in case", and with that one field CUSTREC is part
    /// of our model forever.
    /// </summary>
    public sealed record LeakyCustomer(string Name, CUSTREC Source);
}

namespace FeWoLearning.Architecture.Exercises.CrossCutting
{
    using Ex058.Legacy;
    using Ex058.Sales;

    // Exercise 058 — AntiCorruptionLayer (cross-cutting).
    // Goal:   Talk to another team's model without letting it into yours.
    // Drills: translation between bounded contexts, keeping a foreign model out.
    // Passes: Translate - CUST_NM becomes a trimmed Name; "A"/"S"/"C" become
    //                     AccountStanding; cents become a decimal amount.
    //         unknown   - an unrecognised status code throws, NAMING the code. Defaulting
    //                     it to Active is how a suspended customer starts buying again.
    //         fitness   - FindForeignTypeLeaks reports "LeakyCustomer" and does not report
    //                     "Customer".
    //
    // An honest limit, stated because a fitness check that oversells itself is worse than
    // none: this only catches a foreign TYPE crossing the boundary. A local record that
    // copies CUSTREC field for field - Name, Stat, CredLimCents - passes every assertion
    // here and has imported the other team's model just as completely, in a shape that no
    // reflection can distinguish from a deliberate design. The check catches the
    // mechanical leak; the conceptual one is caught by people reading the code.
    public static class Ex058_AntiCorruptionLayer
    {
        public static Customer Translate(CUSTREC record) =>
            throw new NotImplementedException(
                "TODO: Ex058 - trim the name, map the status code to AccountStanding, and turn cents into a decimal amount");

        /// <summary>
        /// Scan this assembly for types in a namespace ending ".Ex058.Sales" whose public
        /// properties expose a type from ".Ex058.Legacy" - directly or through a generic
        /// argument - and report their names.
        /// </summary>
        public static IReadOnlyList<string> FindForeignTypeLeaks() =>
            throw new NotImplementedException(
                "TODO: Ex058 - report Sales types whose properties expose a Legacy type");
    }
}
