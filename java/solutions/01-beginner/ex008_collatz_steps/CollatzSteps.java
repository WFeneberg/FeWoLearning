package fewolearning.exercises.beginner.ex008_collatz_steps;

/*
Exercise 008 - Collatz steps (reference solution).
*/
public final class CollatzSteps {
    private CollatzSteps() {
    }

    public static int stepsToOne(int start) {
        if (start <= 0) {
            throw new IllegalArgumentException("start must be positive: " + start);
        }
        long current = start;
        int steps = 0;
        while (current != 1) {
            current = isEven(current) ? current / 2 : 3 * current + 1;
            steps++;
        }
        return steps;
    }

    public static boolean isEven(long value) {
        return value % 2 == 0;
    }
}
