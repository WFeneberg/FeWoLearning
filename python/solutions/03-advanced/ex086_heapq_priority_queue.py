"""Exercise 086 — heapq, tuple priorities and tie-breaking (reference solution)."""

import heapq
import itertools
from typing import Any, Generic, TypeVar

T = TypeVar("T")

_REMOVED = object()


class PriorityQueue(Generic[T]):
    def __init__(self) -> None:
        self._heap: list[list[Any]] = []
        self._entry_finder: dict[T, list[Any]] = {}
        self._counter = itertools.count()

    def push(self, item: T, priority: float) -> None:
        if item in self._entry_finder:
            self.remove(item)
        count = next(self._counter)
        entry = [priority, count, item]
        self._entry_finder[item] = entry
        heapq.heappush(self._heap, entry)

    def remove(self, item: T) -> None:
        entry = self._entry_finder.pop(item)
        entry[2] = _REMOVED

    def pop(self) -> T:
        while self._heap:
            _priority, _count, item = heapq.heappop(self._heap)
            if item is not _REMOVED:
                del self._entry_finder[item]
                return item
        raise KeyError("pop from an empty priority queue")

    def peek(self) -> T:
        while self._heap and self._heap[0][2] is _REMOVED:
            heapq.heappop(self._heap)
        if not self._heap:
            raise KeyError("peek from an empty priority queue")
        return self._heap[0][2]

    def __len__(self) -> int:
        return len(self._entry_finder)
