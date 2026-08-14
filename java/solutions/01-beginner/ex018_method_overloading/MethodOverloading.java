package fewolearning.exercises.beginner.ex018_method_overloading;

/*
Exercise 018 - Method overloading (reference solution).
*/
public final class MethodOverloading {
    private MethodOverloading() {
    }

    public static String describe(int value) {
        return "int: " + value;
    }

    public static String describe(double value) {
        return "double: " + value;
    }

    public static String describe(String value) {
        return "String: " + value;
    }

    public static String describe(int value, int precision) {
        return String.format("%." + precision + "f", (double) value);
    }
}
