"""Exercise 046 — frozen and ordered dataclasses (reference solution)."""

from dataclasses import dataclass, field
from typing import Any


@dataclass(frozen=True)
class Money:
    amount: int
    currency: str

    def plus(self, other: "Money") -> "Money":
        if self.currency != other.currency:
            raise ValueError(f"cannot add {self.currency} to {other.currency}")
        # Frozen means a new instance, not an in-place update.
        return Money(self.amount + other.amount, self.currency)


@dataclass(order=True)
class Version:
    # order=True generates the comparisons as if the fields were a tuple, in
    # declaration order — so declaration order *is* the precedence.
    major: int
    minor: int
    patch: int


@dataclass(order=True)
class Priority:
    rank: int
    # compare=False drops the field from both __eq__ and the ordering methods.
    note: str = field(compare=False)


@dataclass(frozen=True, slots=True)
class Coord:
    # slots=True removes __dict__, so setting an undeclared attribute fails loudly
    # instead of silently creating one.
    x: int
    y: int


def sort_versions(versions: list[Any]) -> list[Any]:
    # No key= needed: the generated __lt__ already defines the order.
    return sorted(versions)


def unique_amounts(values: list[Any]) -> int:
    # frozen=True gives a generated __hash__, which is what lets these into a set.
    return len(set(values))
