import pytest

from ex028_any_all import (
    all_positive,
    all_unique,
    any_negative,
    count_consumed,
    first_failing,
    has_digit,
    none_match,
)


@pytest.mark.parametrize(
    "numbers, expected",
    [([1, 2], True), ([1, 0], False), ([-1], False), ([], True)],
)
def test_all_positive(numbers: list[int], expected: bool) -> None:
    assert all_positive(numbers) is expected


@pytest.mark.parametrize(
    "numbers, expected",
    [([1, -2], True), ([1, 2], False), ([0], False), ([], False)],
)
def test_any_negative(numbers: list[int], expected: bool) -> None:
    assert any_negative(numbers) is expected


def test_none_match_when_nothing_matches() -> None:
    assert none_match([1, 3, 5], lambda n: n % 2 == 0) is True


def test_none_match_when_something_matches() -> None:
    assert none_match([1, 2, 3], lambda n: n % 2 == 0) is False


def test_none_match_on_an_empty_input() -> None:
    assert none_match([], lambda n: True) is True


@pytest.mark.parametrize(
    "values, expected",
    [
        (["a", "b"], True),
        (["a", "a"], False),
        ([], True),
        (["x"], True),
        (["a", "b", "a"], False),
    ],
)
def test_all_unique(values: list[str], expected: bool) -> None:
    assert all_unique(values) is expected


@pytest.mark.parametrize(
    "text, expected",
    [("abc1", True), ("abc", False), ("", False), ("123", True), (" 4 ", True)],
)
def test_has_digit(text: str, expected: bool) -> None:
    assert has_digit(text) is expected


def test_first_failing_returns_the_offender() -> None:
    assert first_failing([2, 4, 5, 6], lambda n: n % 2 == 0) == 5


def test_first_failing_when_everything_passes() -> None:
    assert first_failing([2, 4], lambda n: n % 2 == 0) is None


def test_first_failing_on_an_empty_input() -> None:
    assert first_failing([], lambda n: False) is None


def test_first_failing_finds_the_first_not_the_last() -> None:
    assert first_failing([1, 3], lambda n: n % 2 == 0) == 1


def test_count_consumed_short_circuits() -> None:
    found, examined = count_consumed([1, 2, 99, 3, 4])

    assert found is True
    # Stopped at 99, the third item — it did not walk all five.
    assert examined == 3


def test_count_consumed_examines_everything_when_nothing_matches() -> None:
    found, examined = count_consumed([1, 2, 3])

    assert found is False
    assert examined == 3


def test_count_consumed_on_an_empty_list() -> None:
    assert count_consumed([]) == (False, 0)
