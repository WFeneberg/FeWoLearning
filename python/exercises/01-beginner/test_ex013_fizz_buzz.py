import pytest

from ex013_fizz_buzz import (
    apply_rules,
    count_multiples,
    fizz_buzz,
    fizz_buzz_range,
    is_leap_year,
)


@pytest.mark.parametrize(
    "n, expected",
    [
        (1, "1"),
        (3, "Fizz"),
        (5, "Buzz"),
        (15, "FizzBuzz"),
        (30, "FizzBuzz"),
        (9, "Fizz"),
        (10, "Buzz"),
        (0, "FizzBuzz"),
        (-15, "FizzBuzz"),
    ],
)
def test_fizz_buzz(n: int, expected: str) -> None:
    assert fizz_buzz(n) == expected


def test_fizz_buzz_range() -> None:
    assert fizz_buzz_range(1, 6) == ["1", "2", "Fizz", "4", "Buzz"]


@pytest.mark.parametrize("start, stop", [(5, 5), (5, 1)])
def test_fizz_buzz_range_empty(start: int, stop: int) -> None:
    assert fizz_buzz_range(start, stop) == []


@pytest.mark.parametrize(
    "n, rules, expected",
    [
        (15, [(3, "Fizz"), (5, "Buzz")], "FizzBuzz"),
        (3, [(3, "Fizz"), (5, "Buzz")], "Fizz"),
        (7, [(3, "Fizz"), (5, "Buzz")], "7"),
        (30, [(2, "A"), (3, "B"), (5, "C")], "ABC"),
        (4, [(2, "A"), (3, "B")], "A"),
        (1, [], "1"),
    ],
)
def test_apply_rules(n: int, rules: list[tuple[int, str]], expected: str) -> None:
    assert apply_rules(n, rules) == expected


def test_apply_rules_respects_the_given_order() -> None:
    assert apply_rules(15, [(5, "Buzz"), (3, "Fizz")]) == "BuzzFizz"


def test_apply_rules_rejects_a_zero_divisor() -> None:
    with pytest.raises(ValueError):
        apply_rules(10, [(0, "Nope")])


@pytest.mark.parametrize(
    "stop, divisor, expected",
    [(10, 3, 3), (16, 5, 3), (1, 3, 0), (0, 3, 0), (-5, 3, 0), (10, 1, 9)],
)
def test_count_multiples(stop: int, divisor: int, expected: int) -> None:
    assert count_multiples(stop, divisor) == expected


def test_count_multiples_rejects_a_zero_divisor() -> None:
    with pytest.raises(ValueError):
        count_multiples(10, 0)


@pytest.mark.parametrize(
    "year, expected",
    [
        (2024, True),
        (2023, False),
        (1900, False),
        (2000, True),
        (2100, False),
        (1600, True),
    ],
)
def test_is_leap_year(year: int, expected: bool) -> None:
    assert is_leap_year(year) is expected
