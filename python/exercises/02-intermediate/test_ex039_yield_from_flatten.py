from typing import Any

import pytest

from ex039_yield_from_flatten import (
    concat,
    flatten,
    flatten_depth,
    interleave,
    repeat_each,
    walk_tree,
)


def test_concat() -> None:
    assert list(concat([1, 2], [3], [])) == [1, 2, 3]


def test_concat_with_no_arguments() -> None:
    assert list(concat()) == []


def test_concat_yields_items_not_iterables() -> None:
    result = list(concat([1], [2]))
    assert all(isinstance(item, int) for item in result)


def test_concat_accepts_generators() -> None:
    assert list(concat(iter([1]), (n for n in [2, 3]))) == [1, 2, 3]


@pytest.mark.parametrize(
    "nested, expected",
    [
        ([1, [2, [3, (4,)]]], [1, 2, 3, 4]),
        ([[[[5]]]], [5]),
        ([], []),
        ([[], [[]]], []),
        ([1, 2], [1, 2]),
    ],
)
def test_flatten(nested: list[Any], expected: list[Any]) -> None:
    assert list(flatten(nested)) == expected


def test_flatten_treats_strings_as_leaves() -> None:
    assert list(flatten(["ab", ["cd"]])) == ["ab", "cd"]


def test_flatten_mixed_types() -> None:
    assert list(flatten([1, "a", [2, ["b"]]])) == [1, "a", 2, "b"]


@pytest.mark.parametrize(
    "nested, depth, expected",
    [
        ([1, [2, [3]]], 1, [1, 2, [3]]),
        ([1, [2, [3]]], 2, [1, 2, 3]),
        ([1, [2, [3]]], 0, [1, [2, [3]]]),
        ([1, [2, [3]]], 5, [1, 2, 3]),
    ],
)
def test_flatten_depth(nested: list[Any], depth: int, expected: list[Any]) -> None:
    assert list(flatten_depth(nested, depth)) == expected


def test_flatten_depth_rejects_a_negative_depth() -> None:
    with pytest.raises(ValueError):
        list(flatten_depth([1], -1))


TREE: dict[str, Any] = {
    "name": "root",
    "children": [
        {"name": "a", "children": [{"name": "a1"}]},
        {"name": "b"},
    ],
}


def test_walk_tree_is_depth_first() -> None:
    assert list(walk_tree(TREE)) == ["root", "a", "a1", "b"]


def test_walk_tree_leaf_only() -> None:
    assert list(walk_tree({"name": "solo"})) == ["solo"]


def test_walk_tree_with_empty_children() -> None:
    assert list(walk_tree({"name": "x", "children": []})) == ["x"]


@pytest.mark.parametrize(
    "a, b, expected",
    [
        ([1, 3], [2, 4], [1, 2, 3, 4]),
        ([1, 3], [2, 4, 6], [1, 2, 3, 4, 6]),
        ([1, 3, 5], [2], [1, 2, 3, 5]),
        ([], [1, 2], [1, 2]),
        ([1, 2], [], [1, 2]),
        ([], [], []),
    ],
)
def test_interleave(a: list[int], b: list[int], expected: list[int]) -> None:
    assert list(interleave(a, b)) == expected


@pytest.mark.parametrize(
    "values, times, expected",
    [
        ([1, 2], 2, [1, 1, 2, 2]),
        ([1], 3, [1, 1, 1]),
        ([1, 2], 1, [1, 2]),
        ([1, 2], 0, []),
        ([1, 2], -1, []),
        ([], 5, []),
    ],
)
def test_repeat_each(values: list[int], times: int, expected: list[int]) -> None:
    assert list(repeat_each(values, times)) == expected
