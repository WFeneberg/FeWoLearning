"""Exercise 048 — Flag enums (intermediate).

Goal:   Model a set of independent options as one combinable value.
Drills: Flag/IntFlag, bitwise |, &, ~, membership with `in`, iterating a combined
        flag, why the values must be distinct powers of two.
Passes: when `pytest exercises/02-intermediate/test_ex048_enum_flag.py` is green.
"""

from enum import Flag, IntFlag, auto  # noqa: F401  (available for your solution)
from typing import Any


# TODO: make this a Flag whose members are distinct powers of two via auto().
class Permission:
    """READ, WRITE, EXECUTE as combinable flags.

    ``auto()`` on a Flag yields 1, 2, 4 rather than 1, 2, 3 — non-overlapping bits are
    what makes ``READ | WRITE`` a value that still knows both parts.
    """

    READ: int
    WRITE: int
    EXECUTE: int


# TODO: make this an IntFlag.
class FileMode:
    """APPEND=1, BINARY=2, TRUNCATE=4, usable directly as an int bitmask."""

    APPEND: int
    BINARY: int
    TRUNCATE: int


def combine(*flags: Any) -> Any:
    """OR every flag together.

    With no arguments, return the empty Permission (the zero value), not None.
    """
    raise NotImplementedError


def has_all(value: Any, required: Any) -> bool:
    """Report whether `value` contains every bit of `required`.

    ``&`` then compare, or use ``in`` — on a Flag, ``required in value`` means exactly
    this.
    """
    raise NotImplementedError


def has_any(value: Any, candidates: Any) -> bool:
    """Report whether `value` shares at least one bit with `candidates`."""
    raise NotImplementedError


def add(value: Any, flag: Any) -> Any:
    """Return `value` with `flag` set. Setting an already-set flag changes nothing."""
    raise NotImplementedError


def remove(value: Any, flag: Any) -> Any:
    """Return `value` with `flag` cleared. Clearing an unset flag changes nothing."""
    raise NotImplementedError


def to_names(value: Any) -> list[str]:
    """Return the names of the individual flags in `value`, in declaration order.

    Iterating a combined Flag yields its single-bit members. The empty value yields [].
    """
    raise NotImplementedError


def from_names(names: list[str]) -> Any:
    """Build a Permission from member names.

    An unknown name raises KeyError. An empty list yields the empty Permission.
    """
    raise NotImplementedError
