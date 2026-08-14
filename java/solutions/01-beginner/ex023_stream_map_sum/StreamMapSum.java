package fewolearning.exercises.beginner.ex023_stream_map_sum;

import java.util.List;

/*
Exercise 023 - Stream map sum (reference solution).
*/
public final class StreamMapSum {
    private StreamMapSum() {
    }

    public static int sumOfLengths(List<String> values) {
        return values.stream().mapToInt(String::length).sum();
    }

    public static int sumOfSquares(List<Integer> numbers) {
        return numbers.stream().mapToInt(number -> number * number).sum();
    }
}
