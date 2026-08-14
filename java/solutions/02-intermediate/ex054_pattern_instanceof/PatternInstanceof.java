package fewolearning.exercises.intermediate.ex054_pattern_instanceof;

import java.util.List;

/*
Exercise 054 - Pattern matching instanceof (reference solution).
*/
public final class PatternInstanceof {
    private PatternInstanceof() {
    }

    public static String describe(Object value) {
        if (value instanceof String s) {
            return "String[" + s.length() + "]";
        }
        if (value instanceof Integer i) {
            return "Integer[" + i + "]";
        }
        if (value instanceof List<?> list) {
            return "List[" + list.size() + "]";
        }
        return "Unknown";
    }
}
