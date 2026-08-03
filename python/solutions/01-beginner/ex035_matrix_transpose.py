"""Exercise 035 — Matrix operations with nested lists (reference solution)."""

Matrix = list[list[int]]


def transpose(matrix: Matrix) -> Matrix:
    # zip(*matrix) pairs the i-th element of every row; it yields nothing when the
    # rows are empty, which is exactly the documented answer.
    return [list(column) for column in zip(*matrix)]


def zeros(rows: int, cols: int) -> Matrix:
    if rows < 0 or cols < 0:
        raise ValueError("zeros() dimensions must not be negative")
    # A comprehension builds a fresh row each iteration. [[0] * cols] * rows would
    # store the same list object `rows` times.
    return [[0] * cols for _ in range(rows)]


def get_column(matrix: Matrix, index: int) -> list[int]:
    # Indexing each row raises IndexError by itself for an out-of-range column.
    return [row[index] for row in matrix]


def row_sums(matrix: Matrix) -> list[int]:
    return [sum(row) for row in matrix]


def column_sums(matrix: Matrix) -> list[int]:
    if not is_rectangular(matrix):
        raise ValueError("column_sums() needs a rectangular matrix")
    return [sum(column) for column in zip(*matrix)]


def is_rectangular(matrix: Matrix) -> bool:
    # A set of the row lengths has at most one element when they all agree.
    return len({len(row) for row in matrix}) <= 1


def identity(size: int) -> Matrix:
    if size < 0:
        raise ValueError("identity() size must not be negative")
    return [[1 if i == j else 0 for j in range(size)] for i in range(size)]
