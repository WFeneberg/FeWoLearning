package fewolearning.exercises.beginner.ex023_stream_map_sum;

import org.junit.jupiter.api.Test;

import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;

class StreamMapSumTest {

    @Test
    void sumOfLengthsAddsUpEachStringLength() {
        List<String> values = List.of("a", "bb", "ccc");

        assertEquals(6, StreamMapSum.sumOfLengths(values));
    }

    @Test
    void sumOfLengthsOfAnEmptyListIsZero() {
        assertEquals(0, StreamMapSum.sumOfLengths(List.of()));
    }

    @Test
    void sumOfSquaresAddsUpEachSquaredNumber() {
        List<Integer> numbers = List.of(1, 2, 3);

        assertEquals(14, StreamMapSum.sumOfSquares(numbers));
    }

    @Test
    void sumOfSquaresHandlesNegativeNumbers() {
        List<Integer> numbers = List.of(-2, 3);

        assertEquals(13, StreamMapSum.sumOfSquares(numbers));
    }
}
