import pytest

from ex035_matrix_transpose import (
    column_sums,
    get_column,
    identity,
    is_rectangular,
    row_sums,
    transpose,
    zeros,
)


@pytest.mark.parametrize(
    "matrix, expected",
    [
        ([[1, 2], [3, 4]], [[1, 3], [2, 4]]),
        ([[1, 2, 3]], [[1], [2], [3]]),
        ([[1], [2], [3]], [[1, 2, 3]]),
        ([], []),
        ([[], []], []),
    ],
)
def test_transpose(matrix: list[list[int]], expected: list[list[int]]) -> None:
    assert transpose(matrix) == expected


def test_transpose_twice_is_the_original() -> None:
    matrix = [[1, 2, 3], [4, 5, 6]]
    assert transpose(transpose(matrix)) == matrix


def test_zeros_shape() -> None:
    assert zeros(2, 3) == [[0, 0, 0], [0, 0, 0]]


def test_zeros_rows_are_independent() -> None:
    matrix = zeros(2, 2)

    matrix[0][0] = 9

    # With [[0]*2]*2 the second row would also read [9, 0].
    assert matrix == [[9, 0], [0, 0]]


def test_zeros_with_a_zero_dimension() -> None:
    assert zeros(0, 3) == []
    assert zeros(2, 0) == [[], []]


@pytest.mark.parametrize("rows, cols", [(-1, 2), (2, -1)])
def test_zeros_rejects_negative_dimensions(rows: int, cols: int) -> None:
    with pytest.raises(ValueError):
        zeros(rows, cols)


def test_get_column() -> None:
    assert get_column([[1, 2], [3, 4], [5, 6]], 1) == [2, 4, 6]


def test_get_column_first() -> None:
    assert get_column([[1, 2], [3, 4]], 0) == [1, 3]


def test_get_column_out_of_range_raises() -> None:
    with pytest.raises(IndexError):
        get_column([[1, 2]], 5)


def test_get_column_of_an_empty_matrix() -> None:
    assert get_column([], 0) == []


@pytest.mark.parametrize(
    "matrix, expected",
    [([[1, 2], [3, 4]], [3, 7]), ([[5]], [5]), ([], []), ([[]], [0])],
)
def test_row_sums(matrix: list[list[int]], expected: list[int]) -> None:
    assert row_sums(matrix) == expected


@pytest.mark.parametrize(
    "matrix, expected",
    [([[1, 2], [3, 4]], [4, 6]), ([[5]], [5]), ([], []), ([[], []], [])],
)
def test_column_sums(matrix: list[list[int]], expected: list[int]) -> None:
    assert column_sums(matrix) == expected


def test_column_sums_rejects_a_ragged_matrix() -> None:
    with pytest.raises(ValueError):
        column_sums([[1, 2], [3]])


@pytest.mark.parametrize(
    "matrix, expected",
    [
        ([[1, 2], [3, 4]], True),
        ([[1, 2], [3]], False),
        ([], True),
        ([[]], True),
        ([[], [1]], False),
    ],
)
def test_is_rectangular(matrix: list[list[int]], expected: bool) -> None:
    assert is_rectangular(matrix) is expected


@pytest.mark.parametrize(
    "size, expected",
    [
        (1, [[1]]),
        (2, [[1, 0], [0, 1]]),
        (3, [[1, 0, 0], [0, 1, 0], [0, 0, 1]]),
        (0, []),
    ],
)
def test_identity(size: int, expected: list[list[int]]) -> None:
    assert identity(size) == expected


def test_identity_rejects_a_negative_size() -> None:
    with pytest.raises(ValueError):
        identity(-1)
