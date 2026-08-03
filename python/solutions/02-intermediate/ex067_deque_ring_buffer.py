"""Exercise 067 — collections.deque (reference solution)."""

from collections import deque
from typing import Any, Iterable


def ring_buffer(values: Iterable[Any], size: int) -> deque:
    if size < 1:
        raise ValueError("ring_buffer() size must be at least 1")
    # maxlen makes eviction automatic: appending to a full deque drops from the
    # other end, so there is no trimming to forget.
    return deque(values, maxlen=size)


def last_n(values: Iterable[Any], n: int) -> list[Any]:
    if n < 1:
        return []
    # One pass, n items of memory — list(values)[-n:] would hold the entire input.
    return list(deque(values, maxlen=n))


def rotated(values: Iterable[Any], steps: int) -> list[Any]:
    items = deque(values)
    # rotate() is a no-op on an empty deque, so no length guard is needed.
    items.rotate(steps)
    return list(items)


def sliding_windows(values: Iterable[Any], size: int) -> list[tuple[Any, ...]]:
    if size < 1:
        raise ValueError("sliding_windows() size must be at least 1")
    window: deque[Any] = deque(maxlen=size)
    result: list[tuple[Any, ...]] = []
    for value in values:
        window.append(value)
        # The bounded deque *is* the window: appending pushes the oldest item out.
        if len(window) == size:
            result.append(tuple(window))
    return result


class TaskQueue:
    def __init__(self) -> None:
        self._tasks: deque[str] = deque()

    def add(self, task: str) -> None:
        self._tasks.append(task)

    def add_urgent(self, task: str) -> None:
        # O(1) at the front, where list.insert(0, ...) would shift everything.
        self._tasks.appendleft(task)

    def next_task(self) -> str:
        # popleft raises IndexError on an empty deque, which is the documented
        # behaviour, and it is O(1) where list.pop(0) is O(n).
        return self._tasks.popleft()

    def __len__(self) -> int:
        return len(self._tasks)


def drain(queue: TaskQueue) -> list[str]:
    return [queue.next_task() for _ in range(len(queue))]
