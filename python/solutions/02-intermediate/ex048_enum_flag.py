"""Exercise 048 — Flag enums (reference solution)."""

import functools
import operator
from enum import Flag, IntFlag, auto
from typing import Any


class Permission(Flag):
    # On a Flag, auto() allocates successive *bits*: 1, 2, 4 — not 1, 2, 3.
    READ = auto()
    WRITE = auto()
    EXECUTE = auto()


class FileMode(IntFlag):
    APPEND = 1
    BINARY = 2
    TRUNCATE = 4


def combine(*flags: Any) -> Any:
    # Permission(0) is the empty value, and it is the identity for |, so it doubles
    # as the reduce() initial value.
    return functools.reduce(operator.or_, flags, Permission(0))


def has_all(value: Any, required: Any) -> bool:
    # `required in value` is Flag's own spelling of this containment test.
    return (value & required) == required


def has_any(value: Any, candidates: Any) -> bool:
    return bool(value & candidates)


def add(value: Any, flag: Any) -> Any:
    return value | flag


def remove(value: Any, flag: Any) -> Any:
    # &~ clears exactly the bits of `flag` and leaves the rest alone.
    return value & ~flag


def to_names(value: Any) -> list[str]:
    # Iterating a combined Flag yields its single-bit members in declaration order.
    return [member.name for member in value if member.name is not None]


def from_names(names: list[str]) -> Any:
    # Subscripting raises KeyError for an unknown name, which is the documented
    # behaviour.
    return combine(*(Permission[name] for name in names))
