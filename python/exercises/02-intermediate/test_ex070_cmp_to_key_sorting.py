from typing import Any

import pytest

from ex070_cmp_to_key_sorting import (
    compare_lengths,
    is_valid_comparator,
    sort_by_comparator,
    sort_largest_concatenation,
    sort_records,
    sort_version_strings,
)


def test_compare_lengths_sign() -> None:
    assert compare_lengths("a", "bb") < 0
    assert compare_lengths("bb", "a") > 0
    assert compare_lengths("ab", "cd") == 0


def test_sort_by_comparator() -> None:
    assert sort_by_comparator(["ccc", "a", "bb"], compare_lengths) == ["a", "bb", "ccc"]


def test_sort_by_comparator_is_stable_for_equal_elements() -> None:
    assert sort_by_comparator(["bb", "aa"], compare_lengths) == ["bb", "aa"]


def test_sort_by_comparator_empty() -> None:
    assert sort_by_comparator([], compare_lengths) == []


def test_sort_by_comparator_with_a_reversed_comparator() -> None:
    reverse = lambda a, b: compare_lengths(b, a)  # noqa: E731

    assert sort_by_comparator(["a", "ccc", "bb"], reverse) == ["ccc", "bb", "a"]


def test_sort_largest_concatenation_classic_case() -> None:
    assert sort_largest_concatenation([3, 30, 34, 5, 9]) == "9534330"


@pytest.mark.parametrize(
    "numbers, expected",
    [
        ([10, 2], "210"),
        ([1], "1"),
        ([], ""),
        ([0, 0], "00"),
        ([9, 91], "991"),
        ([1, 1, 1], "111"),
    ],
)
def test_sort_largest_concatenation(numbers: list[int], expected: str) -> None:
    assert sort_largest_concatenation(numbers) == expected


def test_sort_version_strings_is_numeric_not_lexical() -> None:
    versions = ["1.10.0", "1.9.0", "1.2.3"]

    # Plain string sorting would put "1.10.0" before "1.9.0".
    assert sort_version_strings(versions) == ["1.2.3", "1.9.0", "1.10.0"]


def test_sort_version_strings_with_differing_segment_counts() -> None:
    assert sort_version_strings(["1.2.1", "1.2", "1.2.0"]) == ["1.2", "1.2.0", "1.2.1"]


def test_sort_version_strings_single() -> None:
    assert sort_version_strings(["2.0"]) == ["2.0"]


def test_sort_version_strings_empty() -> None:
    assert sort_version_strings([]) == []


def test_sort_records() -> None:
    records: list[dict[str, Any]] = [
        {"priority": 1, "name": "b"},
        {"priority": 2, "name": "z"},
        {"priority": 2, "name": "a"},
    ]

    assert sort_records(records) == [
        {"priority": 2, "name": "a"},
        {"priority": 2, "name": "z"},
        {"priority": 1, "name": "b"},
    ]


def test_sort_records_empty() -> None:
    assert sort_records([]) == []


def test_sort_records_single() -> None:
    records: list[dict[str, Any]] = [{"priority": 1, "name": "x"}]

    assert sort_records(records) == records


def test_is_valid_comparator_accepts_a_sound_one() -> None:
    assert is_valid_comparator(compare_lengths, ["a", "bb", "ccc"]) is True


def test_is_valid_comparator_rejects_a_broken_one() -> None:
    # Always claims "greater", which cannot be antisymmetric.
    always_greater = lambda a, b: 1  # noqa: E731

    assert is_valid_comparator(always_greater, ["a", "b"]) is False


def test_is_valid_comparator_on_a_single_sample() -> None:
    assert is_valid_comparator(compare_lengths, ["only"]) is True


def test_is_valid_comparator_on_no_samples() -> None:
    assert is_valid_comparator(compare_lengths, []) is True
