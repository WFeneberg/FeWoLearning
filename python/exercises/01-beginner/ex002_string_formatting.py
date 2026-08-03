"""Exercise 002 — String formatting (beginner).

Goal:   Format values with f-strings and format specifications.
Drills: f-strings, format specs, alignment, rounding.
Passes: when `pytest exercises/01-beginner/test_ex002_string_formatting.py` is green.
"""


def format_price(amount: float, currency: str = "EUR") -> str:
    """Return the amount with exactly two decimals, then a space and the currency.

    ``format_price(3.5)`` -> ``"3.50 EUR"``. Rounding is the usual
    format-spec rounding, so ``2.345`` becomes ``"2.35"``.
    """
    raise NotImplementedError


def format_percent(fraction: float, decimals: int = 1) -> str:
    """Return a fraction as a percentage with `decimals` decimal places.

    ``format_percent(0.1234)`` -> ``"12.3%"``, ``format_percent(0.5, 0)`` -> ``"50%"``.
    """
    raise NotImplementedError


def align_columns(rows: list[tuple[str, int]], width: int) -> list[str]:
    """Return one line per row: the name left-padded to `width`, then the number
    right-aligned in a field of 5.

    ``align_columns([("ab", 7)], 4)`` -> ``["ab  " + "    7"]``. Names longer than
    `width` are not truncated.
    """
    raise NotImplementedError


def thousands(value: int) -> str:
    """Return the integer with a thin-space-free comma group separator.

    ``thousands(1234567)`` -> ``"1,234,567"``.
    """
    raise NotImplementedError
