package fewolearning.exercises.beginner.ex013_switch_expression;

/*
Exercise 013 - Switch expression (reference solution).
*/
public final class SeasonClassifier {
    private SeasonClassifier() {
    }

    public static String seasonForMonth(int month) {
        return switch (month) {
            case 12, 1, 2 -> "Winter";
            case 3, 4, 5 -> "Spring";
            case 6, 7, 8 -> "Summer";
            case 9, 10, 11 -> "Fall";
            default -> throw new IllegalArgumentException("month must be between 1 and 12: " + month);
        };
    }

    public static boolean isSummerMonth(int month) {
        return seasonForMonth(month).equals("Summer");
    }
}
