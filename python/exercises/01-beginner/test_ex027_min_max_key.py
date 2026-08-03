import pytest

from ex027_min_max_key import (
    bounds,
    closest_to,
    highest_scorer,
    largest_by_abs,
    longest,
    longest_line,
    shortest,
)


def test_longest() -> None:
    assert longest(["a", "ccc", "bb"]) == "ccc"


def test_longest_first_wins_on_a_tie() -> None:
    assert longest(["bb", "aa"]) == "bb"


def test_longest_empty() -> None:
    assert longest([]) is None


def test_shortest() -> None:
    assert shortest(["ccc", "a", "bb"]) == "a"


def test_shortest_first_wins_on_a_tie() -> None:
    assert shortest(["bb", "aa", "c"]) == "c"


def test_shortest_empty() -> None:
    assert shortest([]) is None


@pytest.mark.parametrize(
    "numbers, target, expected",
    [
        ([1, 5, 10], 6, 5),
        ([1, 5, 10], 10, 10),
        ([-5, 5], 0, -5),
        ([7], 100, 7),
        ([1, 2, 3], 2, 2),
    ],
)
def test_closest_to(numbers: list[int], target: int, expected: int) -> None:
    assert closest_to(numbers, target) == expected


def test_closest_to_empty() -> None:
    assert closest_to([], 5) is None


def test_highest_scorer() -> None:
    assert highest_scorer({"ada": 10, "grace": 30, "alan": 20}) == "grace"


def test_highest_scorer_first_wins_on_a_tie() -> None:
    assert highest_scorer({"ada": 10, "grace": 10}) == "ada"


def test_highest_scorer_empty() -> None:
    assert highest_scorer({}) is None


@pytest.mark.parametrize(
    "numbers, expected",
    [([1, -9, 3], -9), ([2, 2], 2), ([-1], -1), ([0], 0)],
)
def test_largest_by_abs(numbers: list[int], expected: int) -> None:
    assert largest_by_abs(numbers) == expected


def test_largest_by_abs_empty_uses_the_default() -> None:
    assert largest_by_abs([]) == 0
    assert largest_by_abs([], -1) == -1


@pytest.mark.parametrize(
    "numbers, expected",
    [([3, 1, 2], (1, 3)), ([5], (5, 5)), ([-4, 4], (-4, 4))],
)
def test_bounds(numbers: list[int], expected: tuple[int, int]) -> None:
    assert bounds(numbers) == expected


def test_bounds_empty() -> None:
    assert bounds([]) is None


def test_longest_line() -> None:
    assert longest_line("ab\nlongest line\nc") == "longest line"


def test_longest_line_with_a_trailing_newline() -> None:
    assert longest_line("ab\ncde\n") == "cde"


def test_longest_line_of_empty_text() -> None:
    assert longest_line("") == ""


def test_longest_line_single_line() -> None:
    assert longest_line("only") == "only"
