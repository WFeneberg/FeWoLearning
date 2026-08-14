package fewolearning.exercises.intermediate.ex045_priority_queue_scheduler;

import java.util.ArrayList;
import java.util.List;
import java.util.PriorityQueue;

/*
Exercise 045 - Priority queue scheduler (reference solution).
*/
public final class PriorityQueueScheduler {
    private PriorityQueueScheduler() {
    }

    public record Task(String name, int priority) implements Comparable<Task> {
        @Override
        public int compareTo(Task other) {
            return Integer.compare(other.priority, this.priority);
        }
    }

    public static List<Task> scheduleByPriority(List<Task> tasks) {
        PriorityQueue<Task> queue = new PriorityQueue<>(tasks);
        List<Task> ordered = new ArrayList<>();
        while (!queue.isEmpty()) {
            ordered.add(queue.poll());
        }
        return ordered;
    }
}
