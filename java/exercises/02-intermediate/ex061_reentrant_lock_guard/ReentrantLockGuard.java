package fewolearning.exercises.intermediate.ex061_reentrant_lock_guard;

import java.util.concurrent.locks.ReentrantLock;

/*
Exercise 061 - ReentrantLock guard (intermediate).

Goal:   Guard a critical section with a ReentrantLock, always releasing it.
Drills: ReentrantLock, try/finally.
*/
public final class ReentrantLockGuard {
    private final ReentrantLock lock = new ReentrantLock();
    private int count;

    public void increment() {
        throw new UnsupportedOperationException("TODO");
    }

    public int current() {
        throw new UnsupportedOperationException("TODO");
    }
}
