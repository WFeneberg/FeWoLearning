import math

import pytest

from ex029_math_functions import (
    classify,
    floor_div_and_ceil,
    gcd_of,
    hypotenuse,
    is_square,
    nearly_equal,
    safe_sqrt,
)


@pytest.mark.parametrize(
    "value, expected",
    [
        (2.5, (2, 3)),
        (-2.5, (-3, -2)),
        (3.0, (3, 3)),
        (0.0, (0, 0)),
        (-0.5, (-1, 0)),
    ],
)
def test_floor_div_and_ceil(value: float, expected: tuple[int, int]) -> None:
    assert floor_div_and_ceil(value) == expected


@pytest.mark.parametrize(
    "a, b, expected",
    [(3, 4, 5.0), (0, 0, 0.0), (5, 12, 13.0), (1, 1, math.sqrt(2))],
)
def test_hypotenuse(a: float, b: float, expected: float) -> None:
    assert hypotenuse(a, b) == pytest.approx(expected)


def test_hypotenuse_survives_large_values() -> None:
    # Squaring 1e200 would overflow to inf; hypot must not.
    assert math.isfinite(hypotenuse(1e200, 1e200))


@pytest.mark.parametrize(
    "n, expected",
    [(0, True), (1, True), (4, True), (9, True), (10, False), (-4, False), (2, False)],
)
def test_is_square(n: int, expected: bool) -> None:
    assert is_square(n) is expected


def test_is_square_for_a_large_value() -> None:
    big = 10**20
    assert is_square(big * big) is True
    assert is_square(big * big + 1) is False


def test_nearly_equal_handles_binary_float_error() -> None:
    assert nearly_equal(0.1 + 0.2, 0.3) is True


def test_nearly_equal_rejects_a_real_difference() -> None:
    assert nearly_equal(1.0, 1.1) is False


def test_nearly_equal_honours_the_tolerance() -> None:
    assert nearly_equal(1.0, 1.05, tolerance=0.1) is True


def test_nearly_equal_identical_values() -> None:
    assert nearly_equal(2.0, 2.0) is True


@pytest.mark.parametrize("value, expected", [(4, 2.0), (0, 0.0), (2, math.sqrt(2))])
def test_safe_sqrt(value: float, expected: float) -> None:
    result = safe_sqrt(value)
    assert result is not None
    assert result == pytest.approx(expected)


def test_safe_sqrt_of_a_negative_is_none() -> None:
    assert safe_sqrt(-1) is None


@pytest.mark.parametrize(
    "numbers, expected",
    [([12, 18], 6), ([7, 13], 1), ([10], 10), ([], 0), ([0, 5], 5), ([-12, 18], 6)],
)
def test_gcd_of(numbers: list[int], expected: int) -> None:
    assert gcd_of(numbers) == expected


@pytest.mark.parametrize(
    "value, expected",
    [
        (float("nan"), "nan"),
        (float("inf"), "inf"),
        (float("-inf"), "-inf"),
        (1.5, "finite"),
        (0.0, "finite"),
    ],
)
def test_classify(value: float, expected: str) -> None:
    assert classify(value) == expected
