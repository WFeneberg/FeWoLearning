"""Exercise 014 — Default arguments (beginner).

Goal:   Use default parameters correctly, including the classic mutable-default
        trap that bites everyone once.
Drills: default parameter values, why defaults are evaluated once at definition
        time, None as a sentinel, defaults that depend on other arguments.
Passes: when `pytest exercises/01-beginner/test_ex014_default_arguments.py` is green.
"""


def greet(name: str, greeting: str = "Hello") -> str:
    """Return ``"<greeting>, <name>!"``."""
    raise NotImplementedError


def append_item(item: int, target: list[int] | None = None) -> list[int]:
    """Append `item` to `target`, or to a **fresh** list when none is given.

    This is the mutable-default trap: writing ``target: list[int] = []`` would
    create that list once, at definition time, and every call without an explicit
    target would keep appending to the same one. Use None as the sentinel.
    """
    raise NotImplementedError


def build_config(overrides: dict[str, str] | None = None) -> dict[str, str]:
    """Return the defaults ``{"host": "localhost", "port": "8080"}`` with
    `overrides` applied on top.

    The caller's dict must not be modified, and two calls with no arguments must
    return two independent dicts.
    """
    raise NotImplementedError


def repeat(text: str, times: int = 2, separator: str = " ") -> str:
    """Join `times` copies of `text` with `separator`.

    ``times`` of 0 or less yields ``""``.
    """
    raise NotImplementedError


def slice_window(values: list[int], start: int = 0, length: int | None = None) -> list[int]:
    """Return `length` items starting at `start`; None means "to the end".

    A default that depends on another argument cannot live in the signature, so
    resolve it in the body.
    """
    raise NotImplementedError


def counter_factory(start: int = 0) -> tuple[list[int], int]:
    """Return ``(fresh_list, start)``.

    Exists to prove the returned list is new on every call — never shared.
    """
    raise NotImplementedError
