import itertools
from typing import Callable

import pytest

from ex068_comprehension_vs_generator import (
    any_match,
    count_evaluations,
    first_match,
    is_single_use,
    needs_a_list,
    squares_lazy,
    squares_list,
    sum_lazily,
)


def test_squares_list() -> None:
    result = squares_list([1, 2, 3])

    assert result == [1, 4, 9]
    assert isinstance(result, list)


def test_squares_list_empty() -> None:
    assert squares_list([]) == []


def test_squares_lazy_returns_a_generator() -> None:
    result = squares_lazy([1, 2, 3])

    assert iter(result) is result
    assert not isinstance(result, list)


def test_squares_lazy_values() -> None:
    assert list(squares_lazy([1, 2, 3])) == [1, 4, 9]


def test_squares_lazy_computes_nothing_up_front() -> None:
    # An infinite source is only survivable because nothing is precomputed.
    result = squares_lazy(itertools.count(1))

    assert list(itertools.islice(result, 3)) == [1, 4, 9]


@pytest.mark.parametrize(
    "values, expected",
    [([1, 2, 3, 4], 3), ([1], None), ([], None), ([5, 6], 5)],
)
def test_first_match(values: list[int], expected: int | None) -> None:
    assert first_match(values, lambda n: n > 2) == expected


def test_first_match_stops_early() -> None:
    checked: list[int] = []

    def predicate(n: int) -> bool:
        checked.append(n)
        return n > 1

    first_match([1, 2, 3, 4], predicate)

    # Stopped at 2 rather than testing all four.
    assert checked == [1, 2]


@pytest.mark.parametrize(
    "values, expected",
    [([1, 5], True), ([1, 2], False), ([], False)],
)
def test_any_match(values: list[int], expected: bool) -> None:
    assert any_match(values, lambda n: n > 3) is expected


def test_count_evaluations_short_circuits() -> None:
    found, evaluations = count_evaluations([1, 2, 3, 4, 5], lambda n: n > 2)

    assert found is True
    # Stopped at 3, the third item.
    assert evaluations == 3


def test_count_evaluations_without_a_match_checks_everything() -> None:
    found, evaluations = count_evaluations([1, 2, 3], lambda n: n > 100)

    assert found is False
    assert evaluations == 3


def test_count_evaluations_empty() -> None:
    assert count_evaluations([], lambda n: True) == (False, 0)


@pytest.mark.parametrize(
    "values, expected",
    [([1, 2, 3], 6), ([], 0), ([-1, 1], 0), ([5], 5)],
)
def test_sum_lazily(values: list[int], expected: int) -> None:
    assert sum_lazily(values) == expected


def test_sum_lazily_handles_a_large_range_without_a_list() -> None:
    assert sum_lazily(range(1_000_000)) == 499999500000


def test_is_single_use() -> None:
    first, second = is_single_use([1, 2, 3])

    assert first == [1, 2, 3]
    # A generator is spent after one traversal.
    assert second == []


def test_is_single_use_empty() -> None:
    assert is_single_use([]) == ([], [])


@pytest.mark.parametrize(
    "values, expected",
    [([1, 5, 3], (3, 5)), ([7], (1, 7)), ([-1, -5], (2, -1))],
)
def test_needs_a_list(values: list[int], expected: tuple[int, int]) -> None:
    assert needs_a_list(iter(values)) == expected


def test_needs_a_list_works_on_a_one_shot_iterator() -> None:
    # A generator cannot be walked twice, so the implementation must materialise once.
    assert needs_a_list(n for n in [2, 9, 4]) == (3, 9)
