"""Exercise 031 — Nested data access (beginner).

Goal:   Read and reshape nested dict/list structures without a crash on the way.
Drills: walking mixed containers, defaults, flattening keys, collecting values by
        key at any depth, shallow vs deep copies.
Passes: when `pytest exercises/01-beginner/test_ex031_nested_data_access.py` is green.
"""

from typing import Any


def get_path(data: Any, path: str, default: Any = None) -> Any:
    """Read a dotted path through nested dicts and lists.

    List indices appear as digits: ``"users.0.name"``. A step that does not exist,
    an index out of range, or a non-container on the way yields `default`.
    An empty path returns `data`.
    """
    raise NotImplementedError


def set_path(data: dict[str, Any], path: str, value: Any) -> dict[str, Any]:
    """Write `value` at a dotted path, creating intermediate dicts as needed.

    Mutates and returns `data`. Only dict steps are created — an empty path raises
    ValueError.
    """
    raise NotImplementedError


def flatten_keys(data: dict[str, Any], separator: str = ".") -> dict[str, Any]:
    """Flatten nested dicts into one level, joining keys with `separator`.

    ``{"a": {"b": 1}}`` -> ``{"a.b": 1}``. Lists count as leaf values, not as
    something to descend into. An empty nested dict disappears entirely.
    """
    raise NotImplementedError


def collect_values(data: Any, key: str) -> list[Any]:
    """Return every value stored under `key`, at any depth, in traversal order.

    Descends through both dicts and lists.
    """
    raise NotImplementedError


def deep_merge(base: dict[str, Any], override: dict[str, Any]) -> dict[str, Any]:
    """Merge `override` into `base` recursively, returning a new dict.

    Two dicts at the same key merge; anything else is replaced outright. Neither
    argument may be modified.
    """
    raise NotImplementedError


def count_leaves(data: Any) -> int:
    """Count the non-container values anywhere in the structure.

    Dicts and lists are containers; everything else is a leaf. An empty container
    contributes 0.
    """
    raise NotImplementedError
