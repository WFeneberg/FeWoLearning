package fewolearning.exercises.beginner.ex007_sum_of_digits;

/*
Exercise 007 - Sum of digits (reference solution).
*/
public final class SumOfDigits {
    private SumOfDigits() {
    }

    public static int sumDigits(int value) {
        int remaining = Math.abs(value);
        int sum = 0;
        while (remaining > 0) {
            sum += remaining % 10;
            remaining /= 10;
        }
        return sum;
    }

    public static int digitalRoot(int value) {
        int current = Math.abs(value);
        while (current >= 10) {
            current = sumDigits(current);
        }
        return current;
    }
}
