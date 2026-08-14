package fewolearning.exercises.beginner.ex002_boolean_logic;

/*
Exercise 002 - Boolean logic (reference solution).
*/
public final class BooleanLogic {
    private BooleanLogic() {
    }

    public static boolean canAccess(boolean isAdmin, boolean hasTicket, boolean isBanned) {
        return (isAdmin || hasTicket) && !isBanned;
    }

    public static boolean isInRange(int value, int minimum, int maximum) {
        return value >= minimum && value <= maximum;
    }
}
