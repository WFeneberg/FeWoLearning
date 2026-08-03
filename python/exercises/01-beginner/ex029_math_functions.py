"""Exercise 029 — The math module (beginner).

Goal:   Use math for the things floats get wrong by hand.
Drills: floor/ceil, sqrt, isclose, hypot, gcd, inf/nan, why == is wrong for floats
        and why round() is not floor().
Passes: when `pytest exercises/01-beginner/test_ex029_math_functions.py` is green.
"""


def floor_div_and_ceil(value: float) -> tuple[int, int]:
    """Return ``(floor(value), ceil(value))`` as ints.

    Both round *towards* their side for negatives too: -2.5 floors to -3 and ceils
    to -2, unlike ``int()`` which simply truncates towards zero.
    """
    raise NotImplementedError


def hypotenuse(a: float, b: float) -> float:
    """Return the length of the hypotenuse.

    ``math.hypot`` avoids the overflow that squaring large values would cause.
    """
    raise NotImplementedError


def is_square(n: int) -> bool:
    """Report whether `n` is a perfect square.

    Negative input is False. Use ``math.isqrt``, not ``sqrt``, so large values are
    not misjudged by floating-point error.
    """
    raise NotImplementedError


def nearly_equal(a: float, b: float, tolerance: float = 1e-9) -> bool:
    """Report whether two floats are equal within `tolerance`.

    ``0.1 + 0.2 == 0.3`` is False in binary floating point; this must say True.
    """
    raise NotImplementedError


def safe_sqrt(value: float) -> float | None:
    """Return the square root, or None for a negative input — never raise."""
    raise NotImplementedError


def gcd_of(numbers: list[int]) -> int:
    """Return the greatest common divisor of every number.

    An empty list yields 0, which is the identity for gcd.
    """
    raise NotImplementedError


def classify(value: float) -> str:
    """Return "nan", "inf", "-inf" or "finite".

    NaN is not equal to itself, so ``value == float("nan")`` can never detect it —
    ``math.isnan`` is the only way.
    """
    raise NotImplementedError
