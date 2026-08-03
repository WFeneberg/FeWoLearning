"""Exercise 047 — Enum (intermediate).

Goal:   Replace magic strings and ints with a closed set of named members.
Drills: Enum, auto(), lookup by value vs by name, iteration order, aliases,
        methods on an enum, StrEnum/IntEnum and why they compare to their base type.
Passes: when `pytest exercises/02-intermediate/test_ex047_enum_basics.py` is green.
"""

from enum import Enum, IntEnum, StrEnum, auto  # noqa: F401  (available for your solution)
from typing import Any


# TODO: make this an Enum whose members get automatic values via auto().
class Color:
    """RED, GREEN, BLUE with auto() values, so 1, 2, 3 in declaration order."""

    RED: int
    GREEN: int
    BLUE: int


# TODO: make this an Enum with explicit string values.
class Status:
    """Explicit values: PENDING="pending", ACTIVE="active", DONE="done".

    Also give it a ``is_final`` property that is True only for DONE — an enum can
    carry behaviour, which is most of why it beats bare strings.
    """

    PENDING: str
    ACTIVE: str
    DONE: str


# TODO: make this an IntEnum.
class Level:
    """LOW=1, MEDIUM=5, HIGH=10, comparable with plain ints.

    An IntEnum member *is* an int, so ``Level.LOW < 3`` works. A plain Enum member
    would raise TypeError on that comparison.
    """

    LOW: int
    MEDIUM: int
    HIGH: int


# TODO: make this a StrEnum.
class Suffix:
    """TXT="txt", MD="md" — usable directly where a str is expected."""

    TXT: str
    MD: str


def by_value(value: int) -> Any:  # type: ignore[valid-type]
    """Return the Color member with that value.

    An unknown value raises ValueError, which is what ``Color(value)`` already does.
    """
    raise NotImplementedError


def by_name(name: str) -> Any:  # type: ignore[valid-type]
    """Return the Color member with that name.

    An unknown name raises KeyError, which is what ``Color[name]`` already does. Note
    the asymmetry: lookup by *value* is a call, by *name* is a subscript.
    """
    raise NotImplementedError


def all_names() -> list[str]:
    """Return every Color member's name, in declaration order."""
    raise NotImplementedError


def parse_status(text: str, default: Any = None) -> Any:  # type: ignore[valid-type]
    """Return the Status whose value matches `text`, or `default` when none does.

    Must not raise for an unknown value.
    """
    raise NotImplementedError


def levels_at_least(minimum: Any) -> list[Any]:  # type: ignore[valid-type]
    """Return every Level at or above `minimum`, ascending.

    Relies on IntEnum ordering rather than a key function.
    """
    raise NotImplementedError
