"""Exercise 071 — LRU cache (advanced).

Goal:   Implement a fixed-capacity Least-Recently-Used cache with O(1) get/put.
Drills: OrderedDict / dict ordering, eviction policy, dunder methods.

Do NOT use functools.lru_cache — build the data structure yourself.
"""
from typing import Generic, TypeVar

K = TypeVar("K")
V = TypeVar("V")

_MISSING = object()


class LRUCache(Generic[K, V]):
    def __init__(self, capacity: int) -> None:
        # The capacity guard belongs here too — reject a non-positive capacity with
        # ValueError("capacity must be positive") — but it is left to you, so that
        # every test in this file starts red. A guard shipped in the stub would make
        # the "rejects a bad capacity" test pass against an unimplemented cache.
        raise NotImplementedError

    def get(self, key: K, default: V | None = None) -> V | None:
        """Return the value and mark the key most-recently-used."""
        raise NotImplementedError

    def put(self, key: K, value: V) -> None:
        """Insert/update, evicting the least-recently-used entry if full."""
        raise NotImplementedError

    def __len__(self) -> int:
        raise NotImplementedError
