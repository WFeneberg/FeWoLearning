package fewolearning.exercises.intermediate.ex045_priority_queue_scheduler;

import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class PriorityQueueSchedulerTest {

    @Test
    void compareToOrdersHigherPriorityFirst() {
        PriorityQueueScheduler.Task low = new PriorityQueueScheduler.Task("low", 1);
        PriorityQueueScheduler.Task high = new PriorityQueueScheduler.Task("high", 5);

        assertTrue(high.compareTo(low) < 0);
        assertTrue(low.compareTo(high) > 0);
    }

    @Test
    void scheduleByPriorityReturnsHighestPriorityFirst() {
        PriorityQueueScheduler.Task low = new PriorityQueueScheduler.Task("low", 1);
        PriorityQueueScheduler.Task medium = new PriorityQueueScheduler.Task("medium", 3);
        PriorityQueueScheduler.Task high = new PriorityQueueScheduler.Task("high", 5);

        List<PriorityQueueScheduler.Task> scheduled =
                PriorityQueueScheduler.scheduleByPriority(List.of(low, high, medium));

        assertEquals(List.of(high, medium, low), scheduled);
    }
}
