"""Exercise 088 — WeakValueDictionary and object lifetime (reference solution)."""

import weakref
from typing import Callable, Generic, TypeVar

K = TypeVar("K")
V = TypeVar("V")


class Resource:
    def __init__(self, name: str) -> None:
        self.name = name

    def __repr__(self) -> str:
        return f"Resource({self.name!r})"


class WeakCache(Generic[K, V]):
    def __init__(self) -> None:
        self._values: "weakref.WeakValueDictionary[K, V]" = weakref.WeakValueDictionary()

    def get_or_create(self, key: K, factory: Callable[[], V]) -> V:
        value = self._values.get(key)
        if value is None:
            value = factory()
            self._values[key] = value
        return value

    def __len__(self) -> int:
        return len(self._values)

    def __contains__(self, key: K) -> bool:
        return key in self._values
