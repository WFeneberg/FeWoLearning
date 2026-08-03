"""Exercise 015 — *args and **kwargs (reference solution)."""

from typing import Any, Callable


def total(*values: int) -> int:
    return sum(values)


def largest(*values: int, default: int = 0) -> int:
    # max() raises on an empty iterable unless given a default.
    return max(values, default=default)


def describe(**attributes: str) -> str:
    return ", ".join(f"{key}={value}" for key, value in attributes.items())


def collect(*args: int, **kwargs: int) -> tuple[tuple[int, ...], dict[str, int]]:
    return args, kwargs


def call_with(func: Callable[..., Any], *args: Any, **kwargs: Any) -> Any:
    # The stars unpack on the way back out, so func sees the original arguments
    # rather than one tuple and one dict.
    return func(*args, **kwargs)


def apply_twice(func: Callable[..., int], *args: Any, **kwargs: Any) -> int:
    return func(*args, **kwargs) + func(*args, **kwargs)


def merge_all(*mappings: dict[str, int], **extra: int) -> dict[str, int]:
    result: dict[str, int] = {}
    for mapping in mappings:
        result.update(mapping)
    result.update(extra)
    return result
