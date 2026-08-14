package fewolearning.exercises.intermediate.ex061_reentrant_lock_guard;

import java.util.concurrent.locks.ReentrantLock;

/*
Exercise 061 - ReentrantLock guard (reference solution).
*/
public final class ReentrantLockGuard {
    private final ReentrantLock lock = new ReentrantLock();
    private int count;

    public void increment() {
        lock.lock();
        try {
            count++;
        } finally {
            lock.unlock();
        }
    }

    public int current() {
        lock.lock();
        try {
            return count;
        } finally {
            lock.unlock();
        }
    }
}
