import pytest

from ex025_sort_with_key import (
    by_absolute_value,
    by_last_name,
    by_length,
    by_length_desc,
    case_insensitive,
    sort_in_place,
    top_scores,
)


def test_by_length() -> None:
    assert by_length(["ccc", "a", "bb"]) == ["a", "bb", "ccc"]


def test_by_length_is_stable_for_equal_lengths() -> None:
    assert by_length(["bb", "aa", "c"]) == ["c", "bb", "aa"]


def test_by_length_returns_a_new_list() -> None:
    words = ["bb", "a"]

    result = by_length(words)

    assert result == ["a", "bb"]
    assert words == ["bb", "a"]


def test_by_length_empty() -> None:
    assert by_length([]) == []


def test_by_length_desc() -> None:
    assert by_length_desc(["a", "ccc", "bb"]) == ["ccc", "bb", "a"]


def test_by_length_desc_keeps_ties_in_original_order() -> None:
    # reverse=True is still a stable sort: equal-length items are NOT reversed.
    # Reversing the result with [::-1] would have produced ["aa", "bb"].
    assert by_length_desc(["bb", "aa"]) == ["bb", "aa"]


@pytest.mark.parametrize(
    "words, expected",
    [
        (["Banana", "apple", "Cherry"], ["apple", "Banana", "Cherry"]),
        (["b", "A"], ["A", "b"]),
        ([], []),
    ],
)
def test_case_insensitive(words: list[str], expected: list[str]) -> None:
    assert case_insensitive(words) == expected


@pytest.mark.parametrize(
    "numbers, expected",
    [
        ([-5, 2, -1], [-1, 2, -5]),
        ([3, -3], [3, -3]),
        ([0], [0]),
        ([], []),
    ],
)
def test_by_absolute_value(numbers: list[int], expected: list[int]) -> None:
    assert by_absolute_value(numbers) == expected


def test_by_last_name() -> None:
    names = ["Ada Lovelace", "Grace Hopper", "Alan Turing"]
    assert by_last_name(names) == ["Grace Hopper", "Ada Lovelace", "Alan Turing"]


def test_by_last_name_with_a_single_word() -> None:
    assert by_last_name(["Zoe", "Ada Lovelace"]) == ["Ada Lovelace", "Zoe"]


def test_by_last_name_empty() -> None:
    assert by_last_name([]) == []


def test_sort_in_place() -> None:
    numbers = [3, 1, 2]

    assert sort_in_place(numbers) is None
    assert numbers == [1, 2, 3]


def test_sort_in_place_empty() -> None:
    numbers: list[int] = []
    sort_in_place(numbers)
    assert numbers == []


def test_top_scores() -> None:
    scores = {"ada": 10, "grace": 30, "alan": 20}
    assert top_scores(scores, 2) == ["grace", "alan"]


def test_top_scores_breaks_ties_alphabetically() -> None:
    scores = {"zoe": 10, "ada": 10, "mia": 10}
    assert top_scores(scores, 3) == ["ada", "mia", "zoe"]


def test_top_scores_more_than_available() -> None:
    assert top_scores({"ada": 1}, 5) == ["ada"]


@pytest.mark.parametrize("n", [0, -1])
def test_top_scores_non_positive(n: int) -> None:
    assert top_scores({"ada": 1}, n) == []
