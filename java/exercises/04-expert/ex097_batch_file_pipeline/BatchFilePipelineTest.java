package fewolearning.exercises.expert.ex097_batch_file_pipeline;

import java.util.List;
import java.util.function.Function;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class BatchFilePipelineTest {

    @Test
    void transformsEveryLineWhenNoneFail() {
        List<String> lines = List.of("a", "b", "c", "d", "e");

        BatchFilePipeline.BatchResult result = BatchFilePipeline.process(lines, 2, String::toUpperCase);

        assertEquals(List.of("A", "B", "C", "D", "E"), result.processed());
        assertEquals(List.of(), result.failures());
    }

    @Test
    void recordsFailingLinesWithoutAbortingTheRestOfTheBatch() {
        List<String> lines = List.of("1", "bad", "3", "worse", "5");
        Function<String, String> transform = line -> {
            if (!line.chars().allMatch(Character::isDigit)) {
                throw new IllegalArgumentException("not numeric: " + line);
            }
            return line;
        };

        BatchFilePipeline.BatchResult result = BatchFilePipeline.process(lines, 2, transform);

        assertEquals(List.of("1", "3", "5"), result.processed());
        assertEquals(List.of("bad", "worse"), result.failures());
    }

    @Test
    void aChunkSizeLargerThanTheRemainingLinesStillProcessesTheLastPartialChunk() {
        List<String> lines = List.of("x", "y", "z");

        BatchFilePipeline.BatchResult result = BatchFilePipeline.process(lines, 10, String::toUpperCase);

        assertEquals(List.of("X", "Y", "Z"), result.processed());
        assertEquals(List.of(), result.failures());
    }
}
