"""Exercise 017 — Exception handling (beginner).

Goal:   Catch what you can handle, let the rest propagate, and always clean up.
Drills: try/except/else/finally, catching several types, `as` binding,
        re-raising with a bare `raise`, exception chaining with `from`.
Passes: when `pytest exercises/01-beginner/test_ex017_exception_handling.py` is green.
"""

from typing import Any, Callable


def safe_int(text: str, default: int = 0) -> int:
    """Return `text` as an int, or `default` when it is not a valid integer.

    Only ValueError is a "not a number" answer; anything else must propagate.
    """
    raise NotImplementedError


def safe_divide(a: float, b: float) -> float | None:
    """Return ``a / b``, or None when `b` is zero."""
    raise NotImplementedError


def first_int(values: list[str]) -> int | None:
    """Return the first value that parses as an int, or None when none do."""
    raise NotImplementedError


def parse_pair(text: str) -> tuple[int, int]:
    """Parse ``"3,4"`` into ``(3, 4)``.

    Malformed input raises ValueError whose message starts with
    ``"invalid pair: "`` followed by the original text, and whose ``__cause__`` is
    the underlying error — use ``raise ... from``.
    """
    raise NotImplementedError


def run_with_cleanup(action: Callable[[], Any], log: list[str]) -> str:
    """Call `action` (a zero-argument callable), always appending "cleanup" to `log`.

    Append "ok" before "cleanup" when the call succeeds, and return its result
    converted with ``str``. When it raises, append "error" before "cleanup" and let
    the exception propagate. Use else/finally rather than duplicating the append.
    """
    raise NotImplementedError


def lookup_or_raise(mapping: dict[str, int], key: str) -> int:
    """Return ``mapping[key]``, converting a missing key into a KeyError whose
    message is exactly ``"unknown key: <key>"``.

    Keep the original KeyError as ``__cause__``.
    """
    raise NotImplementedError
