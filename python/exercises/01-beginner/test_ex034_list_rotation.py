import pytest

from ex034_list_rotation import (
    chunk_rotate,
    move_to_front,
    rotate_in_place,
    rotate_left,
    rotate_right,
    swap,
)


@pytest.mark.parametrize(
    "values, amount, expected",
    [
        ([1, 2, 3, 4], 1, [2, 3, 4, 1]),
        ([1, 2, 3, 4], 0, [1, 2, 3, 4]),
        ([1, 2, 3, 4], 4, [1, 2, 3, 4]),
        ([1, 2, 3, 4], 5, [2, 3, 4, 1]),
        ([1, 2, 3, 4], -1, [4, 1, 2, 3]),
        ([1], 3, [1]),
        ([], 2, []),
    ],
)
def test_rotate_left(values: list[int], amount: int, expected: list[int]) -> None:
    assert rotate_left(values, amount) == expected


def test_rotate_left_returns_a_new_list() -> None:
    values = [1, 2, 3]

    result = rotate_left(values, 1)

    assert result is not values
    assert values == [1, 2, 3]


@pytest.mark.parametrize(
    "values, amount, expected",
    [
        ([1, 2, 3, 4], 1, [4, 1, 2, 3]),
        ([1, 2, 3, 4], 5, [4, 1, 2, 3]),
        ([1, 2, 3, 4], -1, [2, 3, 4, 1]),
        ([], 2, []),
    ],
)
def test_rotate_right(values: list[int], amount: int, expected: list[int]) -> None:
    assert rotate_right(values, amount) == expected


def test_rotate_in_place_mutates_the_caller_list() -> None:
    values = [1, 2, 3, 4]

    assert rotate_in_place(values, 1) is None
    assert values == [2, 3, 4, 1]


def test_rotate_in_place_with_a_wrapping_amount() -> None:
    values = [1, 2, 3]
    rotate_in_place(values, 4)
    assert values == [2, 3, 1]


def test_rotate_in_place_empty() -> None:
    values: list[int] = []
    rotate_in_place(values, 2)
    assert values == []


def test_swap() -> None:
    values = [1, 2, 3]

    result = swap(values, 0, 2)

    assert result == [3, 2, 1]
    assert result is values


def test_swap_same_index_is_a_no_op() -> None:
    assert swap([1, 2], 1, 1) == [1, 2]


def test_swap_out_of_range_raises() -> None:
    with pytest.raises(IndexError):
        swap([1, 2], 0, 5)


@pytest.mark.parametrize(
    "values, index, expected",
    [
        ([1, 2, 3, 4], 2, [3, 1, 2, 4]),
        ([1, 2, 3], 0, [1, 2, 3]),
        ([1, 2, 3], -1, [3, 1, 2]),
    ],
)
def test_move_to_front(values: list[int], index: int, expected: list[int]) -> None:
    assert move_to_front(values, index) == expected


def test_move_to_front_does_not_modify_the_input() -> None:
    values = [1, 2, 3]

    move_to_front(values, 2)

    assert values == [1, 2, 3]


def test_move_to_front_out_of_range_raises() -> None:
    with pytest.raises(IndexError):
        move_to_front([1, 2], 5)


@pytest.mark.parametrize(
    "values, size, expected",
    [
        ([1, 2, 3, 4, 5], 2, [2, 1, 4, 3, 5]),
        ([1, 2, 3, 4], 2, [2, 1, 4, 3]),
        ([1, 2, 3], 3, [2, 3, 1]),
        ([1, 2, 3], 1, [1, 2, 3]),
        ([], 2, []),
    ],
)
def test_chunk_rotate(values: list[int], size: int, expected: list[int]) -> None:
    assert chunk_rotate(values, size) == expected


@pytest.mark.parametrize("size", [0, -1])
def test_chunk_rotate_rejects_a_non_positive_size(size: int) -> None:
    with pytest.raises(ValueError):
        chunk_rotate([1, 2], size)
