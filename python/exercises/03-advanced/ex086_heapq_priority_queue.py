"""Exercise 086 — heapq, tuple priorities and tie-breaking (advanced).

Goal:   Build the priority queue `heapq` gives you the pieces for but not the whole
        of: stable tie-breaking between equal priorities, and removing or
        re-prioritizing an item that is already queued.
Drills: `heapq.heappush`/`heappop`, ordering a min-heap of tuples, an insertion-order
        counter as a tiebreaker (so two equal priorities never fall back to
        comparing the items themselves — which may not even support `<`), and lazy
        deletion (mark-and-skip) for O(log n) removal instead of rebuilding the heap.
Passes: when `pytest exercises/03-advanced/test_ex086_heapq_priority_queue.py` is green.

Note:   a heap entry must be a list, not a tuple — the "mark removed" step below
        mutates the item slot in place, and tuples are immutable.
"""

import heapq
import itertools
from typing import Any, Generic, TypeVar

T = TypeVar("T")

_REMOVED = object()


class PriorityQueue(Generic[T]):
    """A min-priority queue: `pop()` always returns the lowest-priority live item.

    Equal priorities come out in the order they were pushed. Pushing an item that
    is already queued re-prioritizes it (the old entry becomes dead weight the heap
    silently skips over) rather than queuing a duplicate.
    """

    def __init__(self) -> None:
        """Set up the heap, a way to find an item's live entry by the item itself,
        and a monotonically increasing counter to break priority ties."""
        raise NotImplementedError

    def push(self, item: T, priority: float) -> None:
        """Queue `item` at `priority`, or re-prioritize it if already queued.

        Build the heap entry as ``[priority, count, item]`` — the counter comes from
        the shared counter, and it must be *unique per push* so entries with equal
        priority still compare deterministically without ever comparing two `item`s
        against each other.
        """
        raise NotImplementedError

    def remove(self, item: T) -> None:
        """Remove `item` from the queue without rebuilding the heap.

        Look up its entry and mark it removed in place (overwrite the item slot
        with the sentinel) — `pop`/`peek` skip entries marked this way. Removing an
        item that is not queued raises KeyError.
        """
        raise NotImplementedError

    def pop(self) -> T:
        """Remove and return the lowest-priority live item.

        Skip past any entries `remove` (or a re-prioritizing `push`) marked dead.
        An empty queue raises KeyError.
        """
        raise NotImplementedError

    def peek(self) -> T:
        """Return the lowest-priority live item without removing it.

        Same dead-entry skipping as `pop` — but skipped entries are still gone for
        good once observed, so do not push them back.
        """
        raise NotImplementedError

    def __len__(self) -> int:
        """The number of live (not removed) items currently queued."""
        raise NotImplementedError
