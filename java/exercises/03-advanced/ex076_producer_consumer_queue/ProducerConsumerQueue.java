package fewolearning.exercises.advanced.ex076_producer_consumer_queue;

import java.util.concurrent.BlockingQueue;

/*
Exercise 076 - Producer/consumer queue (advanced).

Goal:   Coordinate a producer and consumer through a shared BlockingQueue.
Drills: blocking queues, coordination.
*/
public final class ProducerConsumerQueue {
    private ProducerConsumerQueue() {
    }

    public static void produce(BlockingQueue<Integer> queue, int value) throws InterruptedException {
        throw new UnsupportedOperationException("TODO");
    }

    public static int consume(BlockingQueue<Integer> queue) throws InterruptedException {
        throw new UnsupportedOperationException("TODO");
    }
}
