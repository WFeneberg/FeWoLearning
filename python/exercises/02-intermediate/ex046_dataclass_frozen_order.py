"""Exercise 046 — frozen and ordered dataclasses (intermediate).

Goal:   Make value objects immutable, hashable and comparable.
Drills: frozen=True and FrozenInstanceError, hashability as a consequence of
        immutability, order=True and the field order it compares by,
        compare=False to exclude a field, slots=True.
Passes: when `pytest exercises/02-intermediate/test_ex046_dataclass_frozen_order.py` is green.

Note:   as in ex045, the classes carry only annotations. Choosing the right decorator
        arguments is the exercise.
"""

from dataclasses import dataclass, field  # noqa: F401  (available for your solution)
from typing import Any


# TODO: make this an immutable dataclass.
class Money:
    """An immutable amount in minor units.

    Assigning to a field must raise ``dataclasses.FrozenInstanceError``, and instances
    must be usable as dict keys and set members — freezing is what makes the
    generated ``__hash__`` available.
    """

    amount: int
    currency: str

    def plus(self, other: "Money") -> "Money":
        """Return a new Money with the amounts added.

        Different currencies raise ValueError. Since the class is frozen, this has to
        build a new instance rather than mutating self.
        """
        raise NotImplementedError


# TODO: make this an ordered dataclass.
class Version:
    """A semantic version, comparable with <, <=, > and >=.

    Comparison is tuple-like over the fields **in declaration order**: major first,
    then minor, then patch.
    """

    major: int
    minor: int
    patch: int


# TODO: make this ordered, but exclude `note` from comparison and equality.
class Priority:
    """A ranked item whose free-text note must not affect ordering or equality.

    Two Priority values with the same rank are equal even with different notes, which
    is what ``field(compare=False)`` gives you.
    """

    rank: int
    note: str


# TODO: make this a frozen dataclass with slots.
class Coord:
    """A point with ``__slots__``, so unknown attributes cannot be set.

    ``slots=True`` removes ``__dict__``, which means a typo'd attribute name raises
    AttributeError instead of silently creating a new attribute.
    """

    x: int
    y: int


def sort_versions(versions: list[Any]) -> list[Any]:
    """Sort versions ascending, relying on the generated ordering rather than a key."""
    raise NotImplementedError


def unique_amounts(values: list[Any]) -> int:
    """Return how many distinct Money values there are, using a set.

    Only possible because frozen dataclasses are hashable.
    """
    raise NotImplementedError
