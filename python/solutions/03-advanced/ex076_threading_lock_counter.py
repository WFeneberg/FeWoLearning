"""Exercise 076 — threading and locks (reference solution)."""

import threading
from typing import Any, Callable


class Counter:
    def __init__(self) -> None:
        self._value = 0
        self._lock = threading.Lock()

    def increment(self, times: int = 1) -> None:
        # `self._value += times` is read, add, write. A thread switch between the read
        # and the write loses the other thread's update, so the lock is not optional.
        with self._lock:
            self._value += times

    @property
    def value(self) -> int:
        with self._lock:
            return self._value


def run_threads(worker: Callable[[], None], count: int) -> None:
    if count < 1:
        raise ValueError("run_threads() count must be at least 1")
    threads = [threading.Thread(target=worker) for _ in range(count)]
    for thread in threads:
        thread.start()
    # join() is what makes the caller's assertions meaningful; without it the work
    # may still be in flight.
    for thread in threads:
        thread.join()


def total_with_lock(thread_count: int, per_thread: int) -> int:
    counter = Counter()

    def worker() -> None:
        for _ in range(per_thread):
            counter.increment()

    run_threads(worker, thread_count)
    return counter.value


class ReentrantAccount:
    def __init__(self) -> None:
        self._balance = 0
        # RLock, not Lock: deposit_twice() acquires and then calls deposit(), which
        # acquires again on the same thread. A plain Lock would deadlock there.
        self._lock = threading.RLock()

    def deposit(self, amount: int) -> None:
        with self._lock:
            self._balance += amount

    def deposit_twice(self, amount: int) -> None:
        with self._lock:
            self.deposit(amount)
            self.deposit(amount)

    @property
    def balance(self) -> int:
        with self._lock:
            return self._balance


def wait_for_signal() -> list[str]:
    log: list[str] = []
    ready = threading.Event()
    log_lock = threading.Lock()

    def waiter() -> None:
        # wait() blocks until set() is called — no polling, and no sleep to guess at.
        ready.wait()
        with log_lock:
            log.append("waited")

    thread = threading.Thread(target=waiter)
    thread.start()
    with log_lock:
        log.append("signalled")
    ready.set()
    thread.join()
    return log


def thread_local_values(thread_count: int) -> list[int]:
    storage = threading.local()
    results: list[int] = []
    lock = threading.Lock()

    def worker(index: int) -> None:
        # Each thread sees its own `storage.value`; the attribute is not shared.
        storage.value = index
        with lock:
            results.append(storage.value)

    threads = [threading.Thread(target=worker, args=(index,)) for index in range(thread_count)]
    for thread in threads:
        thread.start()
    for thread in threads:
        thread.join()
    # Completion order varies, so sort for a deterministic answer.
    return sorted(results)
