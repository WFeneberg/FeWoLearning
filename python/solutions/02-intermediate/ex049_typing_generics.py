"""Exercise 049 — Generics (reference solution)."""

from typing import Callable, Iterable, Protocol


class Stack[T]:
    def __init__(self) -> None:
        # Built in __init__, not as a class attribute, so instances never share it.
        self._items: list[T] = []

    def push(self, item: T) -> None:
        self._items.append(item)

    def pop(self) -> T:
        if not self._items:
            raise IndexError("pop from an empty stack")
        return self._items.pop()

    def peek(self) -> T:
        if not self._items:
            raise IndexError("peek at an empty stack")
        return self._items[-1]

    def __len__(self) -> int:
        return len(self._items)

    def __bool__(self) -> bool:
        # Without this, truthiness would fall back to __len__ anyway — but stating it
        # keeps the intent explicit.
        return bool(self._items)


class Pair[A, B]:
    def __init__(self, first: A, second: B) -> None:
        self.first = first
        self.second = second

    def swapped(self) -> "Pair[B, A]":
        # The element types swap along with the values, which a type checker tracks.
        return Pair(self.second, self.first)

    def __eq__(self, other: object) -> bool:
        if not isinstance(other, Pair):
            # NotImplemented would also work, but False is enough here and keeps the
            # comparison with a tuple honest.
            return False
        return (self.first, self.second) == (other.first, other.second)

    def __repr__(self) -> str:
        return f"Pair({self.first!r}, {self.second!r})"


class Comparable(Protocol):
    def __lt__(self, other: object, /) -> bool: ...


def first[T](items: Iterable[T], default: T | None = None) -> T | None:
    # next() over iter() consumes exactly one item, so a generator argument keeps the
    # rest of its values.
    return next(iter(items), default)


def largest[T: Comparable](items: Iterable[T]) -> T | None:
    return max(items, default=None)  # type: ignore[type-var]


def group_by[K, V](items: Iterable[V], key: Callable[[V], K]) -> dict[K, list[V]]:
    groups: dict[K, list[V]] = {}
    for item in items:
        # setdefault keeps insertion order both across groups and within them.
        groups.setdefault(key(item), []).append(item)
    return groups
