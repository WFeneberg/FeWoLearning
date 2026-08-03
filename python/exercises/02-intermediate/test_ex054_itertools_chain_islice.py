import itertools
from typing import Any

import pytest

from ex054_itertools_chain_islice import (
    after_false,
    duplicate,
    every_nth,
    first_n,
    flatten_one,
    join,
    pairs,
    while_true,
    window,
)


def test_join() -> None:
    assert list(join([1, 2], [3], [])) == [1, 2, 3]


def test_join_is_lazy() -> None:
    result = join([1], [2])

    assert iter(result) is result
    assert next(result) == 1


def test_join_with_nothing() -> None:
    assert list(join()) == []


def test_flatten_one() -> None:
    assert list(flatten_one([[1, 2], [3]])) == [1, 2, 3]


def test_flatten_one_works_on_a_lazy_outer_stream() -> None:
    outer = ([n, n] for n in itertools.count(1))

    assert list(itertools.islice(flatten_one(outer), 5)) == [1, 1, 2, 2, 3]


def test_flatten_one_empty() -> None:
    assert list(flatten_one([])) == []


def test_window() -> None:
    assert window(iter([0, 1, 2, 3, 4]), 1, 3) == [1, 2]


def test_window_beyond_the_end() -> None:
    assert window(iter([0, 1]), 1, 10) == [1]


def test_window_start_beyond_the_end() -> None:
    assert window(iter([0, 1]), 5, 10) == []


@pytest.mark.parametrize(
    "count, expected",
    [(2, [1, 2]), (0, []), (-1, []), (10, [1, 2, 3])],
)
def test_first_n(count: int, expected: list[int]) -> None:
    assert first_n(iter([1, 2, 3]), count) == expected


@pytest.mark.parametrize(
    "step, expected",
    [(2, [0, 2, 4]), (1, [0, 1, 2, 3, 4]), (3, [0, 3]), (10, [0])],
)
def test_every_nth(step: int, expected: list[int]) -> None:
    assert every_nth(iter([0, 1, 2, 3, 4]), step) == expected


@pytest.mark.parametrize("step", [0, -1])
def test_every_nth_rejects_a_bad_step(step: int) -> None:
    with pytest.raises(ValueError):
        every_nth(iter([1]), step)


def test_while_true_stops_at_the_first_failure() -> None:
    # The 5 stops it, even though 1 would have passed.
    assert while_true([1, 2, 5, 1], lambda n: n < 3) == [1, 2]


def test_while_true_all_pass() -> None:
    assert while_true([1, 2], lambda n: n < 10) == [1, 2]


def test_while_true_none_pass() -> None:
    assert while_true([5, 1], lambda n: n < 3) == []


def test_after_false_returns_everything_after_the_first_failure() -> None:
    # Dropping stops at 5, so the trailing 1 is kept.
    assert after_false([1, 2, 5, 1], lambda n: n < 3) == [5, 1]


def test_after_false_when_nothing_is_dropped() -> None:
    assert after_false([5, 1], lambda n: n < 3) == [5, 1]


def test_after_false_when_everything_is_dropped() -> None:
    assert after_false([1, 2], lambda n: n < 3) == []


@pytest.mark.parametrize(
    "values, expected",
    [
        ([1, 2, 3], [(1, 2), (2, 3)]),
        ([1, 2], [(1, 2)]),
        ([1], []),
        ([], []),
    ],
)
def test_pairs(values: list[Any], expected: list[tuple[Any, Any]]) -> None:
    assert pairs(values) == expected


def test_duplicate_gives_two_full_copies() -> None:
    first, second = duplicate(iter([1, 2, 3]))

    assert first == [1, 2, 3]
    assert second == [1, 2, 3]


def test_duplicate_of_an_empty_iterator() -> None:
    assert duplicate(iter([])) == ([], [])
