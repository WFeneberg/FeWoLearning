"""Exercise 029 — The math module (reference solution)."""

import math


def floor_div_and_ceil(value: float) -> tuple[int, int]:
    # int(-2.5) would be -2 (truncation towards zero); floor is -3.
    return math.floor(value), math.ceil(value)


def hypotenuse(a: float, b: float) -> float:
    # hypot scales internally, so a**2 + b**2 never overflows on the way.
    return math.hypot(a, b)


def is_square(n: int) -> bool:
    if n < 0:
        return False
    # isqrt is exact integer arithmetic; sqrt() would lose precision above 2**53.
    root = math.isqrt(n)
    return root * root == n


def nearly_equal(a: float, b: float, tolerance: float = 1e-9) -> bool:
    return math.isclose(a, b, rel_tol=0.0, abs_tol=tolerance)


def safe_sqrt(value: float) -> float | None:
    if value < 0:
        return None
    return math.sqrt(value)


def gcd_of(numbers: list[int]) -> int:
    # gcd() of no arguments is 0, the identity, so the empty case needs no branch.
    return math.gcd(*numbers)


def classify(value: float) -> str:
    # isnan first: NaN compares unequal to everything, itself included.
    if math.isnan(value):
        return "nan"
    if math.isinf(value):
        return "inf" if value > 0 else "-inf"
    return "finite"
