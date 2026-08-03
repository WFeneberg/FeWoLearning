import pytest

from ex024_counter_most_common import (
    char_counts,
    difference,
    duplicates,
    expand,
    merge_counts,
    most_common_value,
    top_n,
)


def test_char_counts() -> None:
    assert char_counts("aab") == {"a": 2, "b": 1}


def test_char_counts_includes_whitespace() -> None:
    assert char_counts("a b") == {"a": 1, " ": 1, "b": 1}


def test_char_counts_empty() -> None:
    assert char_counts("") == {}


def test_top_n() -> None:
    values = ["a", "b", "a", "c", "a", "b"]
    assert top_n(values, 2) == [("a", 3), ("b", 2)]


def test_top_n_more_than_available() -> None:
    assert top_n(["a"], 5) == [("a", 1)]


@pytest.mark.parametrize("n", [0, -1])
def test_top_n_non_positive(n: int) -> None:
    assert top_n(["a", "b"], n) == []


def test_top_n_breaks_ties_by_first_encounter() -> None:
    # Both appear twice; "b" was seen first, so it comes first.
    assert top_n(["b", "a", "b", "a"], 2) == [("b", 2), ("a", 2)]


def test_most_common_value() -> None:
    assert most_common_value(["a", "b", "a"]) == "a"


def test_most_common_value_empty() -> None:
    assert most_common_value([]) is None


def test_most_common_value_single() -> None:
    assert most_common_value(["only"]) == "only"


@pytest.mark.parametrize(
    "values, expected",
    [
        (["a", "b", "a", "c", "c"], ["a", "c"]),
        (["a", "b"], []),
        ([], []),
        (["z", "z", "z"], ["z"]),
    ],
)
def test_duplicates(values: list[str], expected: list[str]) -> None:
    assert duplicates(values) == expected


def test_merge_counts() -> None:
    assert merge_counts({"a": 1, "b": 2}, {"b": 3, "c": 1}) == {"a": 1, "b": 5, "c": 1}


def test_merge_counts_with_an_empty_side() -> None:
    assert merge_counts({"a": 1}, {}) == {"a": 1}


def test_difference_drops_entries_that_hit_zero() -> None:
    assert difference({"a": 3, "b": 1}, {"a": 1, "b": 1}) == {"a": 2}


def test_difference_drops_entries_that_would_go_negative() -> None:
    assert difference({"a": 1}, {"a": 5}) == {}


def test_difference_ignores_keys_only_in_b() -> None:
    assert difference({"a": 2}, {"z": 1}) == {"a": 2}


def test_expand() -> None:
    assert expand({"a": 2, "b": 1}) == ["a", "a", "b"]


def test_expand_skips_non_positive_counts() -> None:
    assert expand({"a": 2, "b": 0, "c": -1}) == ["a", "a"]


def test_expand_empty() -> None:
    assert expand({}) == []
