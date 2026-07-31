namespace FeWoLearning.Exercises.Beginner;

// Exercise 013 — AgeCalculator (reference solution).
public static class AgeCalculator
{
    public static int GetAge(DateTime birthDate, DateTime referenceDate)
    {
        int age = referenceDate.Year - birthDate.Year;

        // If the birthday hasn't occurred yet this year (relative to the
        // reference date), subtract one. Comparing month then day handles
        // leap-year birthdays (Feb 29) correctly: in a non-leap reference
        // year, DateTime never produces Feb 29, so the month/day comparison
        // still resolves as "birthday not yet reached" until Mar 1.
        if (referenceDate.Month < birthDate.Month ||
            (referenceDate.Month == birthDate.Month && referenceDate.Day < birthDate.Day))
        {
            age--;
        }

        return age;
    }
}
