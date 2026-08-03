from typing import Any

import pytest

from ex053_itertools_groupby import (
    compress,
    count_groups,
    first_of_each_run,
    group_lengths,
    group_sorted,
    longest_run,
    runs,
)


def test_runs_splits_non_adjacent_repeats() -> None:
    assert runs("aabba") == [("a", 2), ("b", 2), ("a", 1)]


def test_runs_all_distinct() -> None:
    assert runs("abc") == [("a", 1), ("b", 1), ("c", 1)]


def test_runs_single_run() -> None:
    assert runs("aaa") == [("a", 3)]


def test_runs_empty() -> None:
    assert runs("") == []


def test_runs_on_numbers() -> None:
    assert runs([1, 1, 2, 1]) == [(1, 2), (2, 1), (1, 1)]


def test_group_sorted_collects_every_equal_key() -> None:
    words = ["apple", "banana", "avocado", "blueberry"]

    # Input is deliberately not grouped by first letter already.
    assert group_sorted(words, lambda w: w[0]) == {
        "a": ["apple", "avocado"],
        "b": ["banana", "blueberry"],
    }


def test_group_sorted_by_a_numeric_key() -> None:
    assert group_sorted([3, 1, 4, 1, 5], lambda n: n % 2) == {0: [4], 1: [3, 1, 1, 5]}


def test_group_sorted_empty() -> None:
    assert group_sorted([], lambda x: x) == {}


def test_group_lengths() -> None:
    words = ["a", "bb", "cc", "d"]

    assert group_lengths(words) == {1: ["a", "d"], 2: ["bb", "cc"]}


def test_group_lengths_empty() -> None:
    assert group_lengths([]) == {}


@pytest.mark.parametrize(
    "text, expected",
    [
        ("aaabb", "a3b2"),
        ("abc", "a1b1c1"),
        ("", ""),
        ("aabaa", "a2b1a2"),
        ("z", "z1"),
    ],
)
def test_compress(text: str, expected: str) -> None:
    assert compress(text) == expected


@pytest.mark.parametrize(
    "values, expected",
    [
        ("aabbbc", ("b", 3)),
        ("abc", ("a", 1)),
        ("aabb", ("a", 2)),
        ([1, 1, 1, 2], (1, 3)),
    ],
)
def test_longest_run(values: Any, expected: tuple[Any, int]) -> None:
    assert longest_run(values) == expected


def test_longest_run_empty() -> None:
    assert longest_run("") is None


def test_first_of_each_run() -> None:
    assert first_of_each_run([1, 1, 2, 2, 1]) == [1, 2, 1]


def test_first_of_each_run_no_duplicates() -> None:
    assert first_of_each_run([1, 2, 3]) == [1, 2, 3]


def test_first_of_each_run_empty() -> None:
    assert first_of_each_run([]) == []


def test_count_groups() -> None:
    assert count_groups([1, 1, 2, 2, 1], lambda n: n) == 3


def test_count_groups_by_parity() -> None:
    # 1,3 odd | 2,4 even | 5 odd  -> three runs
    assert count_groups([1, 3, 2, 4, 5], lambda n: n % 2) == 3


def test_count_groups_empty() -> None:
    assert count_groups([], lambda n: n) == 0


def test_count_groups_single() -> None:
    assert count_groups([7], lambda n: n) == 1
