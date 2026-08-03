"""Exercise 018 — Input validation (beginner).

Goal:   Reject bad input early, with an error that says what was wrong.
Drills: raising ValueError/TypeError with useful messages, guard clauses,
        choosing the right exception type, validating before doing work.
Passes: when `pytest exercises/01-beginner/test_ex018_input_validation.py` is green.
"""


def require_positive(value: int, name: str = "value") -> int:
    """Return `value`, raising ValueError when it is not greater than zero.

    The message must be exactly ``"<name> must be positive, got <value>"``.
    """
    raise NotImplementedError


def require_in_range(value: int, low: int, high: int) -> int:
    """Return `value` when ``low <= value <= high``, else raise ValueError.

    Message: ``"value must be between <low> and <high>, got <value>"``.
    An inverted range (``low > high``) is a caller bug: raise ValueError with
    ``"invalid range: <low> > <high>"``, checked *before* looking at `value`.
    """
    raise NotImplementedError


def require_str(value: object) -> str:
    """Return `value` when it is a str, else raise **TypeError**.

    Message: ``"expected str, got <typename>"``. A wrong *type* is a TypeError; a
    right type with a wrong *value* is a ValueError. That distinction is the point.
    """
    raise NotImplementedError


def require_non_empty(values: list[int]) -> list[int]:
    """Return `values`, raising ValueError with ``"must not be empty"`` when empty."""
    raise NotImplementedError


def parse_age(text: str) -> int:
    """Parse an age between 0 and 150 inclusive.

    Non-numeric input raises ValueError ``"not a number: <text>"``; a number out of
    range raises ValueError ``"age out of range: <value>"``.
    """
    raise NotImplementedError


def average(values: list[float]) -> float:
    """Return the mean.

    An empty list raises ValueError ``"cannot average an empty list"`` rather than
    letting a ZeroDivisionError escape from the arithmetic.
    """
    raise NotImplementedError
