import pytest

from ex033_range_step import (
    arithmetic_series,
    countdown,
    evens_up_to,
    every_nth,
    indices_reversed,
    is_in_range,
    sum_multiples,
)


@pytest.mark.parametrize(
    "limit, expected",
    [(6, [0, 2, 4, 6]), (5, [0, 2, 4]), (0, [0]), (1, [0]), (-1, [])],
)
def test_evens_up_to(limit: int, expected: list[int]) -> None:
    assert evens_up_to(limit) == expected


@pytest.mark.parametrize(
    "start, expected",
    [(3, [3, 2, 1]), (1, [1]), (0, []), (-5, [])],
)
def test_countdown(start: int, expected: list[int]) -> None:
    assert countdown(start) == expected


@pytest.mark.parametrize(
    "values, n, expected",
    [
        (["a", "b", "c", "d", "e"], 2, ["a", "c", "e"]),
        (["a", "b", "c"], 1, ["a", "b", "c"]),
        (["a", "b", "c"], 5, ["a"]),
        ([], 2, []),
    ],
)
def test_every_nth(values: list[str], n: int, expected: list[str]) -> None:
    assert every_nth(values, n) == expected


@pytest.mark.parametrize("n", [0, -1])
def test_every_nth_rejects_a_non_positive_step(n: int) -> None:
    with pytest.raises(ValueError):
        every_nth(["a"], n)


@pytest.mark.parametrize(
    "values, expected",
    [(["a", "b", "c"], [2, 1, 0]), (["a"], [0]), ([], [])],
)
def test_indices_reversed(values: list[str], expected: list[int]) -> None:
    assert indices_reversed(values) == expected


@pytest.mark.parametrize(
    "start, step, count, expected",
    [
        (1, 2, 4, [1, 3, 5, 7]),
        (10, -3, 3, [10, 7, 4]),
        (5, 0, 3, [5, 5, 5]),
        (0, 1, 0, []),
        (0, 1, -1, []),
    ],
)
def test_arithmetic_series(start: int, step: int, count: int, expected: list[int]) -> None:
    assert arithmetic_series(start, step, count) == expected


@pytest.mark.parametrize(
    "value, start, stop, step, expected",
    [
        (4, 0, 10, 2, True),
        (5, 0, 10, 2, False),
        (10, 0, 10, 2, False),  # stop is exclusive
        (0, 0, 10, 2, True),
        (7, 10, 0, -3, True),   # 10, 7, 4, 1
        (2, 10, 0, -3, False),
    ],
)
def test_is_in_range(value: int, start: int, stop: int, step: int, expected: bool) -> None:
    assert is_in_range(value, start, stop, step) is expected


def test_is_in_range_rejects_a_zero_step() -> None:
    with pytest.raises(ValueError):
        is_in_range(1, 0, 10, 0)


@pytest.mark.parametrize(
    "limit, divisor, expected",
    [(10, 3, 18), (10, 5, 5), (1, 3, 0), (0, 3, 0), (-5, 3, 0), (11, 1, 55)],
)
def test_sum_multiples(limit: int, divisor: int, expected: int) -> None:
    assert sum_multiples(limit, divisor) == expected


@pytest.mark.parametrize("divisor", [0, -2])
def test_sum_multiples_rejects_a_non_positive_divisor(divisor: int) -> None:
    with pytest.raises(ValueError):
        sum_multiples(10, divisor)
