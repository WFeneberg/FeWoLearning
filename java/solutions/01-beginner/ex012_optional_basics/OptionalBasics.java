package fewolearning.exercises.beginner.ex012_optional_basics;

import java.util.List;
import java.util.Optional;

/*
Exercise 012 - Optional basics (reference solution).
*/
public final class OptionalBasics {
    private OptionalBasics() {
    }

    public static Optional<String> findFirstLongerThan(List<String> values, int minimumLength) {
        for (String value : values) {
            if (value.length() > minimumLength) {
                return Optional.of(value);
            }
        }
        return Optional.empty();
    }

    public static String describeOrDefault(Optional<String> value, String defaultValue) {
        return value.orElse(defaultValue);
    }
}
