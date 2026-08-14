package fewolearning.exercises.advanced.ex076_producer_consumer_queue;

import java.util.concurrent.BlockingQueue;

/*
Exercise 076 - Producer/consumer queue (reference solution).
*/
public final class ProducerConsumerQueue {
    private ProducerConsumerQueue() {
    }

    public static void produce(BlockingQueue<Integer> queue, int value) throws InterruptedException {
        queue.put(value);
    }

    public static int consume(BlockingQueue<Integer> queue) throws InterruptedException {
        return queue.take();
    }
}
