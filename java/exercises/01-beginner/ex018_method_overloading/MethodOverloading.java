package fewolearning.exercises.beginner.ex018_method_overloading;

/*
Exercise 018 - Method overloading (beginner).

Goal:   Provide overloaded formatters that resolve by parameter type/arity.
Drills: overload resolution, signatures.
*/
public final class MethodOverloading {
    private MethodOverloading() {
    }

    public static String describe(int value) {
        throw new UnsupportedOperationException("TODO");
    }

    public static String describe(double value) {
        throw new UnsupportedOperationException("TODO");
    }

    public static String describe(String value) {
        throw new UnsupportedOperationException("TODO");
    }

    public static String describe(int value, int precision) {
        throw new UnsupportedOperationException("TODO");
    }
}
