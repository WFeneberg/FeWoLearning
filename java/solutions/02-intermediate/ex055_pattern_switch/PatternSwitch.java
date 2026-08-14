package fewolearning.exercises.intermediate.ex055_pattern_switch;

import java.util.List;

/*
Exercise 055 - Pattern matching switch (reference solution).
*/
public final class PatternSwitch {
    private PatternSwitch() {
    }

    public static String describe(Object value) {
        return switch (value) {
            case null -> "Null";
            case String s -> "String[" + s.length() + "]";
            case Integer i -> "Integer[" + i + "]";
            case List<?> list -> "List[" + list.size() + "]";
            default -> "Unknown";
        };
    }
}
