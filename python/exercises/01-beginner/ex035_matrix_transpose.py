"""Exercise 035 — Matrix operations with nested lists (beginner).

Goal:   Work with a list of rows without losing track of which index is which.
Drills: nested indexing, zip(*matrix) for transposition, why [[0]*n]*m is a trap,
        row/column extraction, validating a ragged matrix.
Passes: when `pytest exercises/01-beginner/test_ex035_matrix_transpose.py` is green.
"""

Matrix = list[list[int]]


def transpose(matrix: Matrix) -> Matrix:
    """Swap rows and columns.

    ``transpose([[1, 2], [3, 4]])`` -> ``[[1, 3], [2, 4]]``. An empty matrix, and a
    matrix of empty rows, both yield ``[]``.
    """
    raise NotImplementedError


def zeros(rows: int, cols: int) -> Matrix:
    """Return a `rows` x `cols` matrix of zeros with **independent** rows.

    ``[[0] * cols] * rows`` would repeat the *same* row object, so writing to one
    row would appear to write to all of them. Build each row separately.
    Negative dimensions raise ValueError.
    """
    raise NotImplementedError


def get_column(matrix: Matrix, index: int) -> list[int]:
    """Return column `index` as a list.

    An index outside any row raises IndexError.
    """
    raise NotImplementedError


def row_sums(matrix: Matrix) -> list[int]:
    """Return the sum of each row."""
    raise NotImplementedError


def column_sums(matrix: Matrix) -> list[int]:
    """Return the sum of each column. A ragged matrix raises ValueError."""
    raise NotImplementedError


def is_rectangular(matrix: Matrix) -> bool:
    """Report whether every row has the same length.

    An empty matrix is rectangular.
    """
    raise NotImplementedError


def identity(size: int) -> Matrix:
    """Return the `size` x `size` identity matrix.

    A size of 0 yields ``[]``; a negative size raises ValueError.
    """
    raise NotImplementedError
