import pytest

from ex008_list_comprehension import (
    clamp_all,
    diagonal,
    even_squares,
    flatten_and_filter,
    pairs,
    squares,
    word_lengths,
)


@pytest.mark.parametrize(
    "numbers, expected",
    [([1, 2, 3], [1, 4, 9]), ([], []), ([-2], [4]), ([0], [0])],
)
def test_squares(numbers: list[int], expected: list[int]) -> None:
    assert squares(numbers) == expected


@pytest.mark.parametrize(
    "numbers, expected",
    [([1, 2, 3, 4], [4, 16]), ([1, 3], []), ([], []), ([0, 2], [0, 4])],
)
def test_even_squares(numbers: list[int], expected: list[int]) -> None:
    assert even_squares(numbers) == expected


@pytest.mark.parametrize(
    "numbers, ceiling, expected",
    [
        ([1, 9], 5, [1, 5]),
        ([1, 2], 5, [1, 2]),
        ([7, 8], 5, [5, 5]),
        ([5], 5, [5]),
        ([], 5, []),
    ],
)
def test_clamp_all_keeps_every_item(numbers: list[int], ceiling: int, expected: list[int]) -> None:
    assert clamp_all(numbers, ceiling) == expected


def test_pairs_varies_the_first_input_slowest() -> None:
    assert pairs([1, 2], ["a", "b"]) == [(1, "a"), (1, "b"), (2, "a"), (2, "b")]


@pytest.mark.parametrize(
    "xs, ys, expected",
    [([1, 2], ["a"], [(1, "a"), (2, "a")]), ([], ["a"], []), ([1], [], [])],
)
def test_pairs_edge_cases(
    xs: list[int], ys: list[str], expected: list[tuple[int, str]]
) -> None:
    assert pairs(xs, ys) == expected


@pytest.mark.parametrize(
    "rows, minimum, expected",
    [
        ([[1, 5], [3, 9]], 3, [5, 3, 9]),
        ([[1], [2]], 10, []),
        ([], 0, []),
        ([[], [4]], 0, [4]),
    ],
)
def test_flatten_and_filter(rows: list[list[int]], minimum: int, expected: list[int]) -> None:
    assert flatten_and_filter(rows, minimum) == expected


def test_word_lengths() -> None:
    assert word_lengths("ab cde f") == [("ab", 2), ("cde", 3), ("f", 1)]


def test_word_lengths_collapses_extra_whitespace() -> None:
    assert word_lengths("  a   bb  ") == [("a", 1), ("bb", 2)]


def test_word_lengths_empty() -> None:
    assert word_lengths("") == []


@pytest.mark.parametrize(
    "matrix, expected",
    [
        ([[1, 2], [3, 4]], [1, 4]),
        ([[1, 2, 3], [4, 5, 6], [7, 8, 9]], [1, 5, 9]),
        ([[7]], [7]),
        ([], []),
    ],
)
def test_diagonal(matrix: list[list[int]], expected: list[int]) -> None:
    assert diagonal(matrix) == expected
