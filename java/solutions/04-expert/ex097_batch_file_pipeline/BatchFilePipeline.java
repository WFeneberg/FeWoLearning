package fewolearning.exercises.expert.ex097_batch_file_pipeline;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Function;

/*
Exercise 097 - Batch file pipeline (reference solution).
*/
public final class BatchFilePipeline {
    private BatchFilePipeline() {
    }

    public record BatchResult(List<String> processed, List<String> failures) {
    }

    public static BatchResult process(List<String> lines, int chunkSize, Function<String, String> transform) {
        List<String> processed = new ArrayList<>();
        List<String> failures = new ArrayList<>();

        for (int start = 0; start < lines.size(); start += chunkSize) {
            int end = Math.min(start + chunkSize, lines.size());
            List<String> chunk = lines.subList(start, end);
            for (String line : chunk) {
                try {
                    processed.add(transform.apply(line));
                } catch (RuntimeException e) {
                    failures.add(line);
                }
            }
        }

        return new BatchResult(processed, failures);
    }
}
