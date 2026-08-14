package fewolearning.exercises.beginner.ex004_array_statistics;

/*
Exercise 004 - Array statistics (reference solution).
*/
public final class ArrayStatistics {
    private ArrayStatistics() {
    }

    public static int min(int[] numbers) {
        requireNonEmpty(numbers);
        int result = numbers[0];
        for (int n : numbers) {
            if (n < result) {
                result = n;
            }
        }
        return result;
    }

    public static int max(int[] numbers) {
        requireNonEmpty(numbers);
        int result = numbers[0];
        for (int n : numbers) {
            if (n > result) {
                result = n;
            }
        }
        return result;
    }

    public static double average(int[] numbers) {
        requireNonEmpty(numbers);
        long sum = 0;
        for (int n : numbers) {
            sum += n;
        }
        return (double) sum / numbers.length;
    }

    private static void requireNonEmpty(int[] numbers) {
        if (numbers.length == 0) {
            throw new IllegalArgumentException("numbers must not be empty");
        }
    }
}
