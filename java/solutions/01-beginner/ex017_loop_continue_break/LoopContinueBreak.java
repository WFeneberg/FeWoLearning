package fewolearning.exercises.beginner.ex017_loop_continue_break;

/*
Exercise 017 - Loop continue/break (reference solution).
*/
public final class LoopContinueBreak {
    private LoopContinueBreak() {
    }

    public static int sumOddNumbersUpTo(int limit) {
        int sum = 0;
        for (int i = 1; i <= limit; i++) {
            if (i % 2 == 0) {
                continue;
            }
            sum += i;
        }
        return sum;
    }

    public static int firstMultipleOf(int factor, int startingFrom, int upperBound) {
        int candidate = startingFrom;
        while (candidate <= upperBound) {
            if (candidate % factor == 0) {
                return candidate;
            }
            candidate++;
        }
        return -1;
    }
}
