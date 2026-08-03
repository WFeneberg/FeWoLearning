import pytest

from ex011_enumerate_zip import (
    index_of_first,
    merge_labels,
    numbered_lines,
    positions_of,
    running_totals,
    sum_products,
    unzip,
)


def test_numbered_lines_starts_at_one() -> None:
    assert numbered_lines(["a", "b"]) == ["1: a", "2: b"]


def test_numbered_lines_empty() -> None:
    assert numbered_lines([]) == []


@pytest.mark.parametrize(
    "values, target, expected",
    [
        (["a", "b", "c"], "b", 1),
        (["a", "b", "a"], "a", 0),
        (["a"], "z", -1),
        ([], "a", -1),
    ],
)
def test_index_of_first(values: list[str], target: str, expected: int) -> None:
    assert index_of_first(values, target) == expected


@pytest.mark.parametrize(
    "values, target, expected",
    [
        (["a", "b", "a", "a"], "a", [0, 2, 3]),
        (["a", "b"], "b", [1]),
        (["a"], "z", []),
        ([], "a", []),
    ],
)
def test_positions_of(values: list[str], target: str, expected: list[int]) -> None:
    assert positions_of(values, target) == expected


@pytest.mark.parametrize(
    "a, b, expected",
    [
        ([1, 2, 3], [4, 5, 6], 32),
        ([2], [3], 6),
        ([], [], 0),
        ([1, -1], [1, 1], 0),
    ],
)
def test_sum_products(a: list[int], b: list[int], expected: int) -> None:
    assert sum_products(a, b) == expected


@pytest.mark.parametrize("a, b", [([1, 2], [1]), ([1], [1, 2])])
def test_sum_products_rejects_mismatched_lengths(a: list[int], b: list[int]) -> None:
    with pytest.raises(ValueError):
        sum_products(a, b)


@pytest.mark.parametrize(
    "names, values, expected",
    [
        (["a", "b"], [1, 2], ["a=1", "b=2"]),
        (["a", "b"], [1], ["a=1"]),
        (["a"], [1, 2], ["a=1"]),
        ([], [], []),
    ],
)
def test_merge_labels(names: list[str], values: list[int], expected: list[str]) -> None:
    assert merge_labels(names, values) == expected


def test_unzip() -> None:
    assert unzip([("a", 1), ("b", 2)]) == (["a", "b"], [1, 2])


def test_unzip_single_row() -> None:
    assert unzip([("a", 1)]) == (["a"], [1])


def test_unzip_empty_yields_two_empty_lists() -> None:
    assert unzip([]) == ([], [])


@pytest.mark.parametrize(
    "values, expected",
    [
        ([1, 2, 3], [(1, 1), (2, 3), (3, 6)]),
        ([5], [(5, 5)]),
        ([], []),
        ([1, -1], [(1, 1), (-1, 0)]),
    ],
)
def test_running_totals(values: list[int], expected: list[tuple[int, int]]) -> None:
    assert running_totals(values) == expected
