package fewolearning.exercises.intermediate.ex045_priority_queue_scheduler;

import java.util.List;

/*
Exercise 045 - Priority queue scheduler (intermediate).

Goal:   Schedule tasks by priority, returning them from highest to lowest.
Drills: priority queues, natural ordering.
*/
public final class PriorityQueueScheduler {
    private PriorityQueueScheduler() {
    }

    public record Task(String name, int priority) implements Comparable<Task> {
        @Override
        public int compareTo(Task other) {
            throw new UnsupportedOperationException("TODO");
        }
    }

    public static List<Task> scheduleByPriority(List<Task> tasks) {
        throw new UnsupportedOperationException("TODO");
    }
}
