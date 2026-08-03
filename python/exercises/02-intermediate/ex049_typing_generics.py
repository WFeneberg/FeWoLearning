"""Exercise 049 — Generics (intermediate).

Goal:   Write containers and functions that keep their element type.
Drills: the PEP 695 ``class Box[T]`` / ``def first[T]`` syntax, bounded type
        parameters, generic methods, and why a generic beats `Any`.
Passes: when `pytest exercises/02-intermediate/test_ex049_typing_generics.py` is green.

Note:   the runtime behaviour is what the tests check — type parameters are erased at
        runtime. Write them anyway: they are the point, and `mypy --strict` is the
        other half of the grade.
"""

from typing import Callable, Iterable, Protocol


class Stack[T]:
    """A last-in-first-out stack of `T`.

    Generic in its element type, so ``Stack[int]`` and ``Stack[str]`` are distinct to
    a type checker while sharing one implementation.
    """

    def __init__(self) -> None:
        raise NotImplementedError

    def push(self, item: T) -> None:
        """Add an item to the top."""
        raise NotImplementedError

    def pop(self) -> T:
        """Remove and return the top item. Empty raises IndexError."""
        raise NotImplementedError

    def peek(self) -> T:
        """Return the top item without removing it. Empty raises IndexError."""
        raise NotImplementedError

    def __len__(self) -> int:
        raise NotImplementedError

    def __bool__(self) -> bool:
        """True when the stack holds anything."""
        raise NotImplementedError


class Pair[A, B]:
    """An immutable two-element pair with independent types.

    Exposes the elements as ``self.first`` and ``self.second``.
    """

    def __init__(self, first: A, second: B) -> None:
        raise NotImplementedError

    def swapped(self) -> "Pair[B, A]":
        """Return a new Pair with the elements exchanged — note the flipped types."""
        raise NotImplementedError

    def __eq__(self, other: object) -> bool:
        raise NotImplementedError

    def __repr__(self) -> str:
        """``Pair(1, 'a')``."""
        raise NotImplementedError


class Comparable(Protocol):
    """Anything supporting ``<``."""

    def __lt__(self, other: object, /) -> bool: ...


def first[T](items: Iterable[T], default: T | None = None) -> T | None:
    """Return the first item, or `default` when there is none.

    A generic function: the return type follows the argument's element type instead
    of collapsing to `Any`.
    """
    raise NotImplementedError


def largest[T: Comparable](items: Iterable[T]) -> T | None:
    """Return the largest item, or None when there is none.

    `T` is *bounded* by Comparable, so only orderable element types are accepted.
    """
    raise NotImplementedError


def group_by[K, V](items: Iterable[V], key: Callable[[V], K]) -> dict[K, list[V]]:
    """Group `items` by ``key(item)``, preserving encounter order within each group.

    """
    raise NotImplementedError
