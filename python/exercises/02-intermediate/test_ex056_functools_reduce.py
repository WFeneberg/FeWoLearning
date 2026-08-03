import operator
from typing import Any

import pytest

from ex056_functools_reduce import (
    compose,
    flatten,
    fold_right,
    longest,
    merge_dicts,
    product,
    running,
    total,
)


@pytest.mark.parametrize(
    "values, expected",
    [([1, 2, 3, 4], 24), ([5], 5), ([], 1), ([2, 0, 3], 0), ([-2, 3], -6)],
)
def test_product(values: list[int], expected: int) -> None:
    assert product(values) == expected


@pytest.mark.parametrize(
    "values, start, expected",
    [([1, 2, 3], 0, 6), ([1, 2, 3], 10, 16), ([], 0, 0), ([], 7, 7)],
)
def test_total(values: list[int], start: int, expected: int) -> None:
    assert total(values, start) == expected


def test_total_default_start() -> None:
    assert total([1, 2]) == 3


@pytest.mark.parametrize(
    "words, expected",
    [
        (["a", "ccc", "bb"], "ccc"),
        (["bb", "aa"], "bb"),
        (["only"], "only"),
        ([], ""),
    ],
)
def test_longest(words: list[str], expected: str) -> None:
    assert longest(words) == expected


def test_merge_dicts() -> None:
    assert merge_dicts([{"a": 1}, {"b": 2}, {"a": 3}]) == {"a": 3, "b": 2}


def test_merge_dicts_empty() -> None:
    assert merge_dicts([]) == {}


def test_merge_dicts_does_not_modify_its_inputs() -> None:
    first = {"a": 1}
    second = {"a": 2}

    merge_dicts([first, second])

    assert first == {"a": 1}
    assert second == {"a": 2}


def test_compose_is_left_to_right() -> None:
    add_one = lambda n: n + 1  # noqa: E731
    double = lambda n: n * 2  # noqa: E731

    # add_one first, then double.
    assert compose(add_one, double)(3) == 8
    # double first, then add_one.
    assert compose(double, add_one)(3) == 7


def test_compose_single_function() -> None:
    assert compose(lambda n: n * 3)(2) == 6


def test_compose_nothing_is_the_identity() -> None:
    assert compose()(42) == 42


def test_compose_three_functions() -> None:
    assert compose(lambda n: n + 1, lambda n: n * 2, str)(3) == "8"


def test_flatten() -> None:
    assert flatten([[1, 2], [3], []]) == [1, 2, 3]


def test_flatten_empty() -> None:
    assert flatten([]) == []


def test_flatten_does_not_modify_its_inputs() -> None:
    first = [1]

    flatten([first, [2]])

    assert first == [1]


@pytest.mark.parametrize(
    "values, expected",
    [
        ([1, 2, 3], [1, 3, 6]),
        ([5], [5]),
        ([], []),
        ([1, -1, 2], [1, 0, 2]),
    ],
)
def test_running_with_addition(values: list[int], expected: list[int]) -> None:
    assert running(values, operator.add) == expected


def test_running_with_max() -> None:
    assert running([1, 3, 2], max) == [1, 3, 3]


def test_fold_right_differs_from_folding_left() -> None:
    subtract = lambda a, b: a - b  # noqa: E731

    # 1 - (2 - (3 - 0)) == 2
    assert fold_right([1, 2, 3], subtract, 0) == 2


def test_fold_right_with_an_associative_operation_matches_the_left_fold() -> None:
    assert fold_right([1, 2, 3], operator.add, 0) == 6


def test_fold_right_empty_returns_the_initial() -> None:
    assert fold_right([], operator.add, 7) == 7


def test_fold_right_single_value() -> None:
    assert fold_right([5], lambda a, b: a - b, 1) == 4
