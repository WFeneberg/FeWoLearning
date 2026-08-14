package fewolearning.exercises.intermediate.ex044_deque_stack_queue;

import java.util.ArrayDeque;
import java.util.Deque;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class DequeStackQueueTest {

    @Test
    void usedAsAStackPopsInLastInFirstOutOrder() {
        Deque<Integer> deque = new ArrayDeque<>();

        DequeStackQueue.pushStack(deque, 1);
        DequeStackQueue.pushStack(deque, 2);
        DequeStackQueue.pushStack(deque, 3);

        assertEquals(3, DequeStackQueue.popStack(deque));
        assertEquals(2, DequeStackQueue.popStack(deque));
        assertEquals(1, DequeStackQueue.popStack(deque));
    }

    @Test
    void usedAsAQueueDequeuesInFirstInFirstOutOrder() {
        Deque<Integer> deque = new ArrayDeque<>();

        DequeStackQueue.enqueue(deque, 1);
        DequeStackQueue.enqueue(deque, 2);
        DequeStackQueue.enqueue(deque, 3);

        assertEquals(1, DequeStackQueue.dequeue(deque));
        assertEquals(2, DequeStackQueue.dequeue(deque));
        assertEquals(3, DequeStackQueue.dequeue(deque));
    }
}
