"""Exercise 012 — Digit arithmetic (beginner).

Goal:   Take numbers apart with integer arithmetic rather than string conversion.
Drills: while loops, divmod, floor division, modulo, negative numbers.
Passes: when `pytest exercises/01-beginner/test_ex012_sum_of_digits.py` is green.
"""


def sum_of_digits(number: int) -> int:
    """Return the sum of the decimal digits, ignoring the sign.

    ``sum_of_digits(-123)`` -> ``6``. Use divmod or //-and-%, not ``str(number)``.
    """
    raise NotImplementedError


def digits(number: int) -> list[int]:
    """Return the decimal digits most-significant first, ignoring the sign.

    ``digits(0)`` -> ``[0]``, ``digits(-105)`` -> ``[1, 0, 5]``.
    """
    raise NotImplementedError


def digital_root(number: int) -> int:
    """Repeatedly sum the digits until a single digit remains.

    ``digital_root(9875)`` -> ``2`` (9+8+7+5=29, 2+9=11, 1+1=2). Negative input
    uses its absolute value.
    """
    raise NotImplementedError


def count_digits(number: int) -> int:
    """Return how many decimal digits the number has, ignoring the sign.

    ``count_digits(0)`` -> ``1``.
    """
    raise NotImplementedError


def reverse_number(number: int) -> int:
    """Return the number with its digits reversed, keeping the sign.

    ``reverse_number(-1230)`` -> ``-321``.
    """
    raise NotImplementedError


def to_base(number: int, base: int) -> str:
    """Render a non-negative number in `base` (2–16) using 0-9a-f.

    ``to_base(255, 16)`` -> ``"ff"``, ``to_base(0, 2)`` -> ``"0"``.
    A negative number or a base outside 2–16 raises ValueError.
    """
    raise NotImplementedError
