package fewolearning.exercises.beginner.ex029_regex_validation;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

/*
Exercise 029 - Regex validation (reference solution).

Accepted shape: local-part@domain.tld, where the local part is one or more
of letters/digits/._%+-, and the domain is one or more dot-separated labels
ending in a suffix of at least two letters.
*/
public final class RegexValidation {
    private static final Pattern EMAIL_PATTERN =
            Pattern.compile("^([A-Za-z0-9._%+-]+)@([A-Za-z0-9.-]+\\.[A-Za-z]{2,})$");

    private RegexValidation() {
    }

    public static boolean isValidEmail(String candidate) {
        return candidate != null && EMAIL_PATTERN.matcher(candidate).matches();
    }

    public static String extractDomain(String email) {
        Matcher matcher = EMAIL_PATTERN.matcher(email == null ? "" : email);
        if (!matcher.matches()) {
            throw new IllegalArgumentException("not a valid email: " + email);
        }
        return matcher.group(2);
    }
}
