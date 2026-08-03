"""Exercise 047 — Enum (reference solution)."""

from enum import Enum, IntEnum, StrEnum, auto
from typing import Any


class Color(Enum):
    # auto() counts from 1 in declaration order.
    RED = auto()
    GREEN = auto()
    BLUE = auto()


class Status(Enum):
    PENDING = "pending"
    ACTIVE = "active"
    DONE = "done"

    @property
    def is_final(self) -> bool:
        # Behaviour on the enum itself is most of why this beats bare strings.
        return self is Status.DONE


class Level(IntEnum):
    # An IntEnum member *is* an int, so comparisons and arithmetic with plain ints
    # work; a plain Enum would raise TypeError on `< 3`.
    LOW = 1
    MEDIUM = 5
    HIGH = 10


class Suffix(StrEnum):
    TXT = "txt"
    MD = "md"


def by_value(value: int) -> Any:
    # Calling the class looks up by value and raises ValueError when unknown.
    return Color(value)


def by_name(name: str) -> Any:
    # Subscripting looks up by name and raises KeyError when unknown.
    return Color[name]


def all_names() -> list[str]:
    return [member.name for member in Color]


def parse_status(text: str, default: Any = None) -> Any:
    try:
        return Status(text)
    except ValueError:
        return default


def levels_at_least(minimum: Any) -> list[Any]:
    # sorted() works directly because IntEnum members order like their ints.
    return sorted(level for level in Level if level >= minimum)
