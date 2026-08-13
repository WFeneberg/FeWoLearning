"""Exercise 088 — WeakValueDictionary and object lifetime (advanced).

Goal:   Cache values whose *lifetime* decides eviction, instead of a policy like
        LRU (`ex071`) deciding it. An entry disappears on its own the moment nothing
        else in the program still holds the value — no `put`, no capacity, no
        manual bookkeeping.
Drills: `weakref.WeakValueDictionary`, why the cached values (not the keys) need to
        support weak references, and the gap between "no more strong references"
        and "the slot is actually gone" (a still-live reference keeps it, dropping
        the last one removes it once the garbage collector has run).
Passes: when `pytest exercises/03-advanced/test_ex088_weakref_cache.py` is green.

Note:   `Resource` below is deliberately a plain, fully-implemented class — the
        object being cached is not the exercise, `WeakCache` is.
"""

from typing import Callable, Generic, TypeVar

K = TypeVar("K")
V = TypeVar("V")


class Resource:
    """A weak-referenceable value to cache. Not part of the exercise."""

    def __init__(self, name: str) -> None:
        self.name = name

    def __repr__(self) -> str:
        return f"Resource({self.name!r})"


class WeakCache(Generic[K, V]):
    """A cache backed by a `WeakValueDictionary`.

    A value stays cached for exactly as long as something *else* in the program
    still holds a strong reference to it — the cache itself never keeps one alive.
    """

    def __init__(self) -> None:
        raise NotImplementedError

    def get_or_create(self, key: K, factory: Callable[[], V]) -> V:
        """Return the cached value for `key`.

        If `key` is absent — never cached, or its value was already garbage
        collected — call `factory()`, cache the result, and return it.
        """
        raise NotImplementedError

    def __len__(self) -> int:
        """How many entries are currently live."""
        raise NotImplementedError

    def __contains__(self, key: K) -> bool:
        raise NotImplementedError
