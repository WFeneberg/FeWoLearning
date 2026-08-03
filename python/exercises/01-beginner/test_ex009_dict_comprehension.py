import pytest

from ex009_dict_comprehension import (
    filter_by_value,
    index_of_each,
    invert,
    lengths_by_word,
    unique_lengths,
    upper_keys,
    zip_to_dict,
)


def test_lengths_by_word() -> None:
    assert lengths_by_word(["ab", "cde"]) == {"ab": 2, "cde": 3}


def test_lengths_by_word_with_a_repeat() -> None:
    assert lengths_by_word(["ab", "ab"]) == {"ab": 2}


def test_lengths_by_word_empty() -> None:
    assert lengths_by_word([]) == {}


def test_invert() -> None:
    assert invert({"a": 1, "b": 2}) == {1: "a", 2: "b"}


def test_invert_last_key_wins_on_a_shared_value() -> None:
    assert invert({"a": 1, "b": 1}) == {1: "b"}


def test_invert_empty() -> None:
    assert invert({}) == {}


@pytest.mark.parametrize(
    "scores, minimum, expected",
    [
        ({"a": 1, "b": 5}, 3, {"b": 5}),
        ({"a": 3}, 3, {"a": 3}),
        ({"a": 1}, 10, {}),
        ({}, 0, {}),
    ],
)
def test_filter_by_value(scores: dict[str, int], minimum: int, expected: dict[str, int]) -> None:
    assert filter_by_value(scores, minimum) == expected


@pytest.mark.parametrize(
    "keys, values, expected",
    [
        (["a", "b"], [1, 2], {"a": 1, "b": 2}),
        (["a", "b"], [1], {"a": 1}),
        (["a"], [1, 2], {"a": 1}),
        ([], [], {}),
    ],
)
def test_zip_to_dict(keys: list[str], values: list[int], expected: dict[str, int]) -> None:
    assert zip_to_dict(keys, values) == expected


def test_upper_keys() -> None:
    assert upper_keys({"a": 1, "bc": 2}) == {"A": 1, "BC": 2}


def test_upper_keys_empty() -> None:
    assert upper_keys({}) == {}


@pytest.mark.parametrize(
    "words, expected",
    [
        (["a", "bb", "cc"], {1, 2}),
        ([], set()),
        (["x"], {1}),
    ],
)
def test_unique_lengths(words: list[str], expected: set[int]) -> None:
    assert unique_lengths(words) == expected


def test_index_of_each_keeps_the_first_occurrence() -> None:
    assert index_of_each(["a", "b", "a"]) == {"a": 0, "b": 1}


def test_index_of_each_without_repeats() -> None:
    assert index_of_each(["x", "y"]) == {"x": 0, "y": 1}


def test_index_of_each_empty() -> None:
    assert index_of_each([]) == {}


def test_index_of_each_all_the_same() -> None:
    assert index_of_each(["z", "z", "z"]) == {"z": 0}
