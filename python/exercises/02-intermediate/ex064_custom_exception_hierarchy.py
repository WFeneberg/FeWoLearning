"""Exercise 064 — Custom exception hierarchies (intermediate).

Goal:   Design exceptions callers can catch at the granularity they need.
Drills: a package-level base class, subclass hierarchies, exceptions carrying data,
        `raise … from` versus `from None`, __cause__ vs __context__, catching a base
        to handle a whole family.
Passes: when `pytest exercises/02-intermediate/test_ex064_custom_exception_hierarchy.py` is green.

Note:   every class below currently inherits straight from Exception. **Wiring up the
        hierarchy is part of the exercise** — re-parent them so the relationships each
        docstring describes actually hold.
"""

from typing import Any


class AppError(Exception):
    """Base for every error this module raises.

    Having one base lets a caller write ``except AppError`` to catch the whole family
    without also swallowing unrelated bugs like a TypeError.
    """


class ValidationError(Exception):
    """A value failed validation.

    Must become a subclass of **AppError**. Carries `field` and `value` as attributes,
    and its message must read ``"invalid <field>: <value>"``.
    """

    def __init__(self, field: str, value: Any) -> None:
        raise NotImplementedError


class MissingFieldError(Exception):
    """A required field was absent entirely.

    Must become a subclass of **ValidationError**, so ``except ValidationError`` also
    catches it. Its message reads ``"missing field: <field>"`` and `value` is None.
    """

    def __init__(self, field: str) -> None:
        raise NotImplementedError


class NotFoundError(Exception):
    """A lookup found nothing.

    Must become a subclass of **AppError**. Carries `key`; message ``"not found: <key>"``.
    """

    def __init__(self, key: str) -> None:
        raise NotImplementedError


def validate_age(value: Any) -> int:
    """Return `value` as an int between 0 and 150.

    A non-numeric value raises ValidationError for field "age" with the **original**
    error as ``__cause__``. A numeric value out of range raises ValidationError with
    **no** cause — use ``from None``, because there is no underlying error to point at.
    """
    raise NotImplementedError


def require_field(data: dict[str, Any], field: str) -> Any:
    """Return ``data[field]``, raising MissingFieldError when absent.

    The original KeyError must be suppressed with ``from None``: it adds nothing a
    caller could act on, and it clutters the traceback.
    """
    raise NotImplementedError


def lookup(store: dict[str, int], key: str) -> int:
    """Return ``store[key]``, raising NotFoundError when absent.

    Keep the KeyError as ``__cause__`` here, so the traceback still shows where the
    lookup happened.
    """
    raise NotImplementedError


def classify(error: Exception) -> str:
    """Return "validation", "missing", "notfound", "app" or "other".

    Order matters: MissingFieldError is a ValidationError, so the more specific check
    has to come first.
    """
    raise NotImplementedError
