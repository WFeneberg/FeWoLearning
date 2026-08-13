"""Exercise 098 — combined TTL + LRU eviction (reference solution)."""

from collections import OrderedDict
from typing import Callable, Generic, TypeVar

K = TypeVar("K")
V = TypeVar("V")

_MISSING = object()


class TTLLRUCache(Generic[K, V]):
    def __init__(self, capacity: int, ttl: float, clock: Callable[[], float]) -> None:
        if capacity <= 0:
            raise ValueError(f"capacity must be positive, got {capacity}")
        if ttl <= 0:
            raise ValueError(f"ttl must be positive, got {ttl}")
        self._capacity = capacity
        self._ttl = ttl
        self._clock = clock
        self._data: OrderedDict[K, tuple[V, float]] = OrderedDict()

    def put(self, key: K, value: V) -> None:
        if key in self._data:
            del self._data[key]
        elif len(self._data) >= self._capacity:
            self._data.popitem(last=False)
        self._data[key] = (value, self._clock() + self._ttl)

    def get(self, key: K, default: V | None = None) -> V | None:
        entry = self._data.get(key, _MISSING)
        if entry is _MISSING:
            return default
        value, expires_at = entry  # type: ignore[misc]
        if self._clock() >= expires_at:
            del self._data[key]
            return default
        self._data.move_to_end(key)
        return value

    def __contains__(self, key: K) -> bool:
        entry = self._data.get(key, _MISSING)
        if entry is _MISSING:
            return False
        _value, expires_at = entry  # type: ignore[misc]
        if self._clock() >= expires_at:
            del self._data[key]
            return False
        return True

    def __len__(self) -> int:
        return len(self._data)
