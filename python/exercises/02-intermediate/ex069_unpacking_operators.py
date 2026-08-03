"""Exercise 069 — Unpacking operators (intermediate).

Goal:   Use * and ** to spread and merge instead of building containers by hand.
Drills: * in calls and literals, ** for dict merging and keyword forwarding,
        merge precedence, star-targets in assignment, and where * is not allowed.
Passes: when `pytest exercises/02-intermediate/test_ex069_unpacking_operators.py` is green.
"""

from typing import Any, Callable, Iterable


def call_with_list(func: Callable[..., Any], args: list[Any]) -> Any:
    """Call `func` with the list spread into positional arguments."""
    raise NotImplementedError


def call_with_dict(func: Callable[..., Any], kwargs: dict[str, Any]) -> Any:
    """Call `func` with the dict spread into keyword arguments."""
    raise NotImplementedError


def concat_lists(*lists: list[Any]) -> list[Any]:
    """Concatenate lists using * inside a list literal.

    ``[*a, *b]`` reads better than ``a + b`` once there are more than two, and it
    accepts any iterable rather than only lists.
    """
    raise NotImplementedError


def merge(*mappings: dict[str, Any]) -> dict[str, Any]:
    """Merge dicts left to right, later values winning, using ** in a literal.

    No input may be modified.
    """
    raise NotImplementedError


def merge_with_extra(base: dict[str, Any], **extra: Any) -> dict[str, Any]:
    """Merge `base` with keyword arguments, the keywords winning."""
    raise NotImplementedError


def split_first_rest(values: Iterable[Any]) -> tuple[Any, list[Any]]:
    """Split into the first item and the rest via a star-target.

    An empty input raises ValueError.
    """
    raise NotImplementedError


def split_ends(values: list[Any]) -> tuple[Any, list[Any], Any]:
    """Split into ``(first, middle, last)``.

    Needs at least two items; fewer raises ValueError.
    """
    raise NotImplementedError


def to_set_literal(*iterables: Iterable[Any]) -> set[Any]:
    """Build one set from several iterables using * inside a set literal.

    With no arguments the result is the empty set — note that ``{}`` is a dict, so an
    empty set literal does not exist and ``set()`` is needed.
    """
    raise NotImplementedError


def forward_all(func: Callable[..., Any], args: list[Any], kwargs: dict[str, Any]) -> Any:
    """Forward both forms in one call."""
    raise NotImplementedError
