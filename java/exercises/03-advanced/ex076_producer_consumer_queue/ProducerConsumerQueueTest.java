package fewolearning.exercises.advanced.ex076_producer_consumer_queue;

import java.util.concurrent.BlockingQueue;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.atomic.AtomicInteger;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class ProducerConsumerQueueTest {

    @Test
    void producedValuesAreConsumedInFifoOrder() throws InterruptedException {
        BlockingQueue<Integer> queue = new LinkedBlockingQueue<>();

        ProducerConsumerQueue.produce(queue, 1);
        ProducerConsumerQueue.produce(queue, 2);
        ProducerConsumerQueue.produce(queue, 3);

        assertEquals(1, ProducerConsumerQueue.consume(queue));
        assertEquals(2, ProducerConsumerQueue.consume(queue));
        assertEquals(3, ProducerConsumerQueue.consume(queue));
    }

    @Test
    void aConsumerThreadBlocksUntilAProducerThreadAddsAValue() throws InterruptedException {
        BlockingQueue<Integer> queue = new LinkedBlockingQueue<>();
        CountDownLatch consumerStarted = new CountDownLatch(1);
        AtomicInteger received = new AtomicInteger();

        Thread consumer = new Thread(() -> {
            consumerStarted.countDown();
            try {
                received.set(ProducerConsumerQueue.consume(queue));
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
            }
        });
        consumer.start();
        consumerStarted.await();

        ProducerConsumerQueue.produce(queue, 42);
        consumer.join();

        assertEquals(42, received.get());
    }
}
