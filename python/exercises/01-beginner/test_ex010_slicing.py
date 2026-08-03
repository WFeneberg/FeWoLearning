import pytest

from ex010_slicing import (
    chunk,
    every_other,
    first_n,
    last_n,
    middle,
    replace_slice,
    reversed_copy,
    shallow_copy,
)


@pytest.mark.parametrize(
    "values, n, expected",
    [
        ([1, 2, 3, 4], 2, [1, 2]),
        ([1, 2], 5, [1, 2]),
        ([1, 2], 0, []),
        ([1, 2], -1, []),
        ([], 3, []),
    ],
)
def test_first_n(values: list[int], n: int, expected: list[int]) -> None:
    assert first_n(values, n) == expected


@pytest.mark.parametrize(
    "values, n, expected",
    [
        ([1, 2, 3, 4], 2, [3, 4]),
        ([1, 2], 5, [1, 2]),
        ([1, 2], 0, []),
        ([1, 2], -1, []),
        ([], 3, []),
    ],
)
def test_last_n(values: list[int], n: int, expected: list[int]) -> None:
    assert last_n(values, n) == expected


@pytest.mark.parametrize(
    "values, expected",
    [
        ([1, 2, 3, 4], [2, 3]),
        ([1, 2, 3], [2]),
        ([1, 2], []),
        ([1], []),
        ([], []),
    ],
)
def test_middle(values: list[int], expected: list[int]) -> None:
    assert middle(values) == expected


@pytest.mark.parametrize(
    "values, expected",
    [([1, 2, 3, 4, 5], [1, 3, 5]), ([1, 2], [1]), ([1], [1]), ([], [])],
)
def test_every_other(values: list[int], expected: list[int]) -> None:
    assert every_other(values) == expected


def test_reversed_copy() -> None:
    values = [1, 2, 3]

    assert reversed_copy(values) == [3, 2, 1]
    assert values == [1, 2, 3]


def test_reversed_copy_empty() -> None:
    assert reversed_copy([]) == []


def test_shallow_copy_is_equal_but_not_identical() -> None:
    values = [1, 2]

    copy = shallow_copy(values)

    assert copy == values
    assert copy is not values


def test_shallow_copy_is_independent() -> None:
    values = [1, 2]
    copy = shallow_copy(values)

    copy.append(3)

    assert values == [1, 2]


def test_replace_slice_same_length() -> None:
    values = [1, 2, 3, 4]

    result = replace_slice(values, 1, 3, [9, 9])

    assert result == [1, 9, 9, 4]
    assert result is values


def test_replace_slice_shorter_replacement() -> None:
    values = [1, 2, 3, 4]
    assert replace_slice(values, 1, 3, [0]) == [1, 0, 4]


def test_replace_slice_longer_replacement() -> None:
    values = [1, 2, 3]
    assert replace_slice(values, 1, 2, [7, 8, 9]) == [1, 7, 8, 9, 3]


def test_replace_slice_with_nothing_deletes() -> None:
    values = [1, 2, 3]
    assert replace_slice(values, 0, 2, []) == [3]


@pytest.mark.parametrize(
    "values, size, expected",
    [
        ([1, 2, 3, 4, 5], 2, [[1, 2], [3, 4], [5]]),
        ([1, 2, 3, 4], 2, [[1, 2], [3, 4]]),
        ([1], 3, [[1]]),
        ([], 2, []),
        ([1, 2, 3], 1, [[1], [2], [3]]),
    ],
)
def test_chunk(values: list[int], size: int, expected: list[list[int]]) -> None:
    assert chunk(values, size) == expected


@pytest.mark.parametrize("size", [0, -1])
def test_chunk_rejects_a_non_positive_size(size: int) -> None:
    with pytest.raises(ValueError):
        chunk([1, 2], size)
