package fewolearning.exercises.intermediate.ex044_deque_stack_queue;

import java.util.Deque;

/*
Exercise 044 - Deque stack/queue (reference solution).
*/
public final class DequeStackQueue {
    private DequeStackQueue() {
    }

    public static void pushStack(Deque<Integer> deque, int value) {
        deque.addFirst(value);
    }

    public static int popStack(Deque<Integer> deque) {
        return deque.removeFirst();
    }

    public static void enqueue(Deque<Integer> deque, int value) {
        deque.addLast(value);
    }

    public static int dequeue(Deque<Integer> deque) {
        return deque.removeFirst();
    }
}
