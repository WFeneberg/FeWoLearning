"""Exercise 015 — *args and **kwargs (beginner).

Goal:   Accept a variable number of arguments and pass them on unchanged.
Drills: *args, **kwargs, unpacking at the call site, forwarding to a wrapped
        callable, why *args is a tuple and **kwargs a dict.
Passes: when `pytest exercises/01-beginner/test_ex015_args_kwargs.py` is green.
"""

from typing import Any, Callable


def total(*values: int) -> int:
    """Return the sum of every positional argument. No arguments yields 0."""
    raise NotImplementedError


def largest(*values: int, default: int = 0) -> int:
    """Return the largest positional argument, or `default` when none were given.

    `default` is keyword-only because it comes after *values.
    """
    raise NotImplementedError


def describe(**attributes: str) -> str:
    """Render keyword arguments as ``"key=value"`` pairs joined by ", ".

    Insertion order is preserved. No arguments yields ``""``.
    """
    raise NotImplementedError


def collect(*args: int, **kwargs: int) -> tuple[tuple[int, ...], dict[str, int]]:
    """Return the two containers as-is, to make their types visible.

    `args` is a tuple, `kwargs` a dict.
    """
    raise NotImplementedError


def call_with(func: Callable[..., Any], *args: Any, **kwargs: Any) -> Any:
    """Invoke `func` with exactly the arguments given, forwarding both forms."""
    raise NotImplementedError


def apply_twice(func: Callable[..., int], *args: Any, **kwargs: Any) -> int:
    """Call `func` twice with the same arguments and return the sum of the results."""
    raise NotImplementedError


def merge_all(*mappings: dict[str, int], **extra: int) -> dict[str, int]:
    """Merge every mapping left to right, then apply `extra` on top.

    Later values win. No input yields ``{}``, and no argument may be modified.
    """
    raise NotImplementedError
