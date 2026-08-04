package fewolearning.exercises.advanced.ex073_spliterator_batching;

import java.util.List;
import java.util.Spliterator;
import java.util.function.Consumer;

/*
Exercise 073 - Spliterator batching (advanced).

Goal:   Implement a Spliterator that yields elements in fixed-size batches.
Drills: custom traversal, characteristics.
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
        throw new UnsupportedOperationException("TODO");
    }

    @Override
    public Spliterator<List<T>> trySplit() {
        throw new UnsupportedOperationException("TODO");
    }

    @Override
    public long estimateSize() {
        throw new UnsupportedOperationException("TODO");
    }

    @Override
    public int characteristics() {
        throw new UnsupportedOperationException("TODO");
    }
}
