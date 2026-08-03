"""Exercise 067 — collections.deque (intermediate).

Goal:   Use the right container for queue-shaped work.
Drills: deque append/appendleft/pop/popleft, maxlen as a self-evicting ring buffer,
        rotate, and why list.pop(0) is O(n) while deque.popleft() is O(1).
Passes: when `pytest exercises/02-intermediate/test_ex067_deque_ring_buffer.py` is green.
"""

from collections import deque
from typing import Any, Iterable


def ring_buffer(values: Iterable[Any], size: int) -> deque:
    """Return a deque with ``maxlen=size`` filled from `values`.

    Once full, each append silently drops the item at the other end — no manual
    trimming. A `size` below 1 raises ValueError.
    """
    raise NotImplementedError


def last_n(values: Iterable[Any], n: int) -> list[Any]:
    """Return the last `n` items of a possibly huge iterable.

    A bounded deque does this in one pass with constant memory, where ``list(values)[-n:]``
    would hold everything. An `n` below 1 yields [].
    """
    raise NotImplementedError


def rotated(values: Iterable[Any], steps: int) -> list[Any]:
    """Return the values rotated right by `steps`, as a list.

    ``deque.rotate`` is positive-to-the-right. A negative `steps` rotates left, and
    the amount may exceed the length.
    """
    raise NotImplementedError


def sliding_windows(values: Iterable[Any], size: int) -> list[tuple[Any, ...]]:
    """Return every consecutive window of `size` items.

    ``sliding_windows([1, 2, 3, 4], 2)`` -> ``[(1, 2), (2, 3), (3, 4)]``. Fewer items
    than `size` yields []. A bounded deque is the natural window: appending pushes the
    oldest item out. A `size` below 1 raises ValueError.
    """
    raise NotImplementedError


class TaskQueue:
    """A FIFO queue that can also take priority work at the front.

    ``add`` appends, ``add_urgent`` prepends, ``next_task`` pops from the left, and
    ``__len__`` reports the size. Popping an empty queue raises IndexError.
    """

    def __init__(self) -> None:
        raise NotImplementedError

    def add(self, task: str) -> None:
        raise NotImplementedError

    def add_urgent(self, task: str) -> None:
        raise NotImplementedError

    def next_task(self) -> str:
        raise NotImplementedError

    def __len__(self) -> int:
        raise NotImplementedError


def drain(queue: TaskQueue) -> list[str]:
    """Pop everything, returning the tasks in the order they came out."""
    raise NotImplementedError
