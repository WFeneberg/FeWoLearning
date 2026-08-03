"""Exercise 045 — dataclasses (intermediate).

Goal:   Let @dataclass generate the boilerplate, and know exactly what it generates.
Drills: @dataclass, field defaults, default_factory for mutable defaults,
        __post_init__ validation, field(init=False)/repr=False, asdict/replace.
Passes: when `pytest exercises/02-intermediate/test_ex045_dataclass_basics.py` is green.

Note:   the classes below carry only **annotations**, not the decorator. Annotations
        alone create no ``__init__`` and no ``__eq__``, so ``Point(1, 2)`` is a
        TypeError until you add ``@dataclass``. Adding it — and the field options and
        ``__post_init__`` each class needs — is the exercise.
"""

from dataclasses import dataclass, field  # noqa: F401  (available for your solution)
from typing import Any


# TODO: make this a dataclass.
class Point:
    """A 2-D point. `y` defaults to 0.

    Two points with equal coordinates must compare equal, and the repr must read
    ``Point(x=1, y=2)`` — both come free with the decorator.
    """

    x: int
    y: int = 0


# TODO: make this a dataclass whose `items` defaults to a fresh empty list.
class Basket:
    """Holds item names.

    ``items: list[str] = []`` is a hard error in a dataclass — it refuses the shared
    mutable default outright. Use ``field(default_factory=list)`` so each instance
    gets its own list.
    """

    items: list[str]

    def add(self, name: str) -> "Basket":
        """Append `name` and return self, for chaining."""
        raise NotImplementedError


# TODO: make this a dataclass and validate in __post_init__.
class Temperature:
    """A temperature in Celsius, rejecting anything below absolute zero.

    Validation belongs in ``__post_init__``, which runs after the generated
    ``__init__`` has assigned the fields. Raise ValueError for celsius < -273.15.
    """

    celsius: float

    @property
    def fahrenheit(self) -> float:
        """Return the temperature in Fahrenheit."""
        raise NotImplementedError


# TODO: make this a dataclass; `slug` is derived, `secret` is hidden from the repr.
class User:
    """A user whose `slug` is computed rather than passed in.

    `slug` must use ``field(init=False)`` so it is not an ``__init__`` parameter, and
    be set in ``__post_init__`` to the lowercased name with spaces turned into
    hyphens. `secret` must use ``field(repr=False)`` so it stays out of the repr.
    """

    name: str
    secret: str
    slug: str


def to_dict(instance: Any) -> dict[str, Any]:
    """Convert a dataclass instance to a nested dict, using ``dataclasses.asdict``."""
    raise NotImplementedError


def with_changes(instance: Any, **changes: Any) -> Any:
    """Return a copy with `changes` applied, using ``dataclasses.replace``.

    The original must be untouched, and ``__post_init__`` runs again on the copy, so
    an invalid change raises.
    """
    raise NotImplementedError
