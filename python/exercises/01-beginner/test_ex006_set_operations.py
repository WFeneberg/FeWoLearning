import pytest

from ex006_set_operations import (
    common_elements,
    dedupe_keep_order,
    group_key,
    has_duplicates,
    is_subset,
    only_in_first,
    symmetric_difference,
)


@pytest.mark.parametrize(
    "values, expected",
    [
        (["b", "a", "b", "c", "a"], ["b", "a", "c"]),
        ([], []),
        (["x"], ["x"]),
        (["x", "x", "x"], ["x"]),
    ],
)
def test_dedupe_keep_order(values: list[str], expected: list[str]) -> None:
    assert dedupe_keep_order(values) == expected


def test_common_elements() -> None:
    assert common_elements([1, 2, 3, 3], [3, 4, 2]) == {2, 3}


def test_common_elements_disjoint() -> None:
    assert common_elements([1], [2]) == set()


def test_only_in_first() -> None:
    assert only_in_first([1, 2, 3], [3]) == {1, 2}


def test_only_in_first_is_not_symmetric() -> None:
    assert only_in_first([1], [1, 2]) == set()


def test_symmetric_difference() -> None:
    assert symmetric_difference([1, 2], [2, 3]) == {1, 3}


def test_symmetric_difference_identical_lists() -> None:
    assert symmetric_difference([1, 2], [2, 1]) == set()


@pytest.mark.parametrize(
    "small, large, expected",
    [
        ([1, 2], [1, 2, 3], True),
        ([], [1], True),
        ([], [], True),
        ([1, 9], [1, 2], False),
        ([1, 1], [1], True),
    ],
)
def test_is_subset(small: list[int], large: list[int], expected: bool) -> None:
    assert is_subset(small, large) is expected


@pytest.mark.parametrize(
    "values, expected",
    [([1, 2, 3], False), ([1, 2, 1], True), ([], False), ([7], False)],
)
def test_has_duplicates(values: list[int], expected: bool) -> None:
    assert has_duplicates(values) is expected


def test_group_key_is_order_independent() -> None:
    assert group_key(["a", "b"]) == group_key(["b", "a"])


def test_group_key_ignores_repeats() -> None:
    assert group_key(["a", "a", "b"]) == group_key(["a", "b"])


def test_group_key_is_usable_as_a_dict_key() -> None:
    buckets: dict[frozenset[str], int] = {}
    buckets[group_key(["x", "y"])] = 1
    buckets[group_key(["y", "x"])] = 2

    assert buckets == {frozenset({"x", "y"}): 2}


def test_group_key_distinguishes_different_tag_sets() -> None:
    assert group_key(["a"]) != group_key(["a", "b"])
