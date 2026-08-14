package fewolearning.exercises.advanced.ex073_spliterator_batching;

import java.util.ArrayList;
import java.util.List;
import java.util.Spliterator;
import java.util.function.Consumer;

/*
Exercise 073 - Spliterator batching (reference solution).
*/
public final class BatchingSpliterator<T> implements Spliterator<List<T>> {
    private final Spliterator<T> source;
    private final int batchSize;

    public BatchingSpliterator(Spliterator<T> source, int batchSize) {
        this.source = source;
        this.batchSize = batchSize;
    }

    @Override
    public boolean tryAdvance(Consumer<? super List<T>> action) {
        List<T> batch = new ArrayList<>(batchSize);
        while (batch.size() < batchSize && source.tryAdvance(batch::add)) {
            // keep pulling elements from the source until the batch fills up
            // or the source runs out.
        }
        if (batch.isEmpty()) {
            return false;
        }
        action.accept(batch);
        return true;
    }

    @Override
    public Spliterator<List<T>> trySplit() {
        return null;
    }

    @Override
    public long estimateSize() {
        long sourceSize = source.estimateSize();
        if (sourceSize == Long.MAX_VALUE) {
            return Long.MAX_VALUE;
        }
        return (sourceSize + batchSize - 1) / batchSize;
    }

    @Override
    public int characteristics() {
        return source.characteristics() & Spliterator.ORDERED;
    }
}
