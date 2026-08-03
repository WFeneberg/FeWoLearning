"""Exercise 076 — threading and locks (advanced).

Goal:   Share state between real OS threads without corrupting it.
Drills: Thread, join, Lock as a context manager, RLock for reentrancy, Event for
        signalling, thread-local storage, and why `+= 1` is not atomic.
Passes: when `pytest exercises/03-advanced/test_ex076_threading_lock_counter.py` is green.

Note:   these are real threads, so a test cannot assert a *specific* interleaving.
        What it can assert is the invariant: with a lock the total is always exact.
"""

import threading
from typing import Any, Callable


class Counter:
    """A thread-safe counter.

    ``value += 1`` is a read, an add and a write — three steps a thread switch can
    interrupt — so every mutation has to hold the lock.
    """

    def __init__(self) -> None:
        raise NotImplementedError

    def increment(self, times: int = 1) -> None:
        """Add `times` to the value, atomically."""
        raise NotImplementedError

    @property
    def value(self) -> int:
        """The current value."""
        raise NotImplementedError


def run_threads(worker: Callable[[], None], count: int) -> None:
    """Start `count` threads running `worker` and wait for all of them.

    A `count` below 1 raises ValueError.
    """
    raise NotImplementedError


def total_with_lock(thread_count: int, per_thread: int) -> int:
    """Have `thread_count` threads each increment a shared Counter `per_thread` times.

    Returns the final value, which must be exactly ``thread_count * per_thread``.
    """
    raise NotImplementedError


class ReentrantAccount:
    """Demonstrates why RLock exists.

    ``deposit`` takes the lock; ``deposit_twice`` takes it and then calls ``deposit``.
    With a plain Lock that second acquisition would deadlock against itself — an RLock
    lets the *same* thread re-enter.
    """

    def __init__(self) -> None:
        raise NotImplementedError

    def deposit(self, amount: int) -> None:
        raise NotImplementedError

    def deposit_twice(self, amount: int) -> None:
        """Deposit `amount` twice, calling `deposit` from inside the held lock."""
        raise NotImplementedError

    @property
    def balance(self) -> int:
        raise NotImplementedError


def wait_for_signal() -> list[str]:
    """Coordinate two threads with an Event.

    A waiter thread appends "waited" once the event is set; the main thread appends
    "signalled" just before setting it. Returns the log, which must be
    ``["signalled", "waited"]`` — the Event guarantees that ordering where a bare flag
    plus a sleep would not.
    """
    raise NotImplementedError


def thread_local_values(thread_count: int) -> list[int]:
    """Give each thread its own value in a ``threading.local``, and collect them.

    Each thread stores its own index and reads it back; the results prove the storage
    is per-thread rather than shared. Returned sorted, since completion order varies.
    """
    raise NotImplementedError
