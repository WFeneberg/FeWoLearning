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
    using System.Reflection;
    using Ex058.Legacy;
    using Ex058.Sales;

    // Exercise 058 — AntiCorruptionLayer (reference solution).
    public static class Ex058_AntiCorruptionLayer
    {
        private const string SalesSuffix = ".Ex058.Sales";
        private const string LegacySuffix = ".Ex058.Legacy";

        public static Customer Translate(CUSTREC record)
        {
            ArgumentNullException.ThrowIfNull(record);

            var standing = record.CUST_STAT switch
            {
                "A" => AccountStanding.Active,
                "S" => AccountStanding.Suspended,
                "C" => AccountStanding.Closed,
                // Not `_ => Active`. Defaulting an unrecognised code to the permissive
                // value is how a suspended customer starts buying again after the other
                // team adds a status nobody told us about.
                _ => throw new ArgumentOutOfRangeException(nameof(record), record.CUST_STAT,
                        $"Unknown customer status code '{record.CUST_STAT}'."),
            };

            return new Customer(
                record.CUST_NM.Trim(),
                standing,
                record.CRED_LIM_CENTS / 100m);
        }

        public static IReadOnlyList<string> FindForeignTypeLeaks()
        {
            var assembly = typeof(Ex058_AntiCorruptionLayer).Assembly;

            var leaks = new List<string>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace?.EndsWith(SalesSuffix, StringComparison.Ordinal) != true)
                    continue;

                if (type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => ExposesLegacy(p.PropertyType)))
                    leaks.Add(type.Name);
            }

            leaks.Sort(StringComparer.Ordinal);
            return leaks;
        }

        private static bool ExposesLegacy(Type type)
        {
            if (type.Namespace?.EndsWith(LegacySuffix, StringComparison.Ordinal) == true)
                return true;

            if (type.IsArray)
                return ExposesLegacy(type.GetElementType()!);

            return type.IsGenericType && type.GetGenericArguments().Any(ExposesLegacy);
        }
    }
}
