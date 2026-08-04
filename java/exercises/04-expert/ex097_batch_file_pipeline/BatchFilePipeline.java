package fewolearning.exercises.expert.ex097_batch_file_pipeline;

import java.util.List;
import java.util.function.Function;

/*
Exercise 097 - Batch file pipeline (expert).

Goal:   Process input lines in fixed-size chunks, reporting failures without aborting the batch.
Drills: chunked processing, fault reporting.
*/
public final class BatchFilePipeline {
    private BatchFilePipeline() {
    }

    public record BatchResult(List<String> processed, List<String> failures) {
    }

    public static BatchResult process(List<String> lines, int chunkSize, Function<String, String> transform) {
        throw new UnsupportedOperationException("TODO");
    }
}
