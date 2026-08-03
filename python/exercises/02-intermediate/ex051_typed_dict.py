"""Exercise 051 — TypedDict (intermediate).

Goal:   Give a dict-shaped payload a checkable schema without turning it into a class.
Drills: TypedDict, total=False for optional keys, Required/NotRequired, nested
        TypedDicts, and the fact that a TypedDict is a plain dict at runtime.
Passes: when `pytest exercises/02-intermediate/test_ex051_typed_dict.py` is green.
"""

from typing import Any, NotRequired, TypedDict


class Address(TypedDict):
    """A required-everything nested payload."""

    street: str
    city: str


class UserRecord(TypedDict):
    """`id` and `name` are required; `email` and `address` are not.

    ``NotRequired`` marks individual keys optional while the rest stay mandatory —
    finer-grained than ``total=False``, which would make *everything* optional.
    """

    id: int
    name: str
    email: NotRequired[str]
    address: NotRequired[Address]


class Partial(TypedDict, total=False):
    """Every key optional, via ``total=False``."""

    a: int
    b: str


def make_user(user_id: int, name: str, email: str | None = None) -> UserRecord:
    """Build a UserRecord, including `email` **only** when one was given.

    Setting ``email=None`` would violate the declared ``str`` type, so an absent email
    means an absent key.
    """
    raise NotImplementedError


def is_valid_user(value: Any) -> bool:
    """Report whether `value` looks like a UserRecord at runtime.

    A TypedDict is just a dict once the program runs — ``isinstance(x, UserRecord)``
    is a TypeError, so the check has to be written by hand: required keys present and
    correctly typed, optional keys correctly typed when present, no unknown keys.
    """
    raise NotImplementedError


def display_name(user: UserRecord) -> str:
    """Return ``"<name> <<email>>"`` when an email is present, else just the name.

    Use ``.get()`` or ``in`` — indexing an absent NotRequired key raises KeyError.
    """
    raise NotImplementedError


def city_of(user: UserRecord, default: str = "unknown") -> str:
    """Return the nested address city, or `default` when there is no address."""
    raise NotImplementedError


def merge_partial(base: Partial, override: Partial) -> Partial:
    """Merge two Partial payloads, `override` winning. Neither input may be modified."""
    raise NotImplementedError


def required_keys() -> set[str]:
    """Return UserRecord's required key names.

    ``TypedDict`` exposes this as ``__required_keys__``.
    """
    raise NotImplementedError
