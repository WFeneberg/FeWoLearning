import pytest

from ex004_list_operations import (
    append_all,
    flatten_once,
    insert_sorted,
    pop_at,
    remove_first,
    sort_in_place_desc,
)


def test_append_all_mutates_and_returns_the_same_object() -> None:
    target = [1, 2]

    result = append_all(target, [3, 4])

    assert result == [1, 2, 3, 4]
    assert result is target


def test_append_all_with_no_values_is_a_no_op() -> None:
    target = [1]
    assert append_all(target, []) == [1]


@pytest.mark.parametrize(
    "values, value, expected",
    [
        ([1, 3, 5], 4, [1, 3, 4, 5]),
        ([], 1, [1]),
        ([2, 4], 1, [1, 2, 4]),
        ([2, 4], 9, [2, 4, 9]),
        ([1, 2, 2, 3], 2, [1, 2, 2, 2, 3]),
    ],
)
def test_insert_sorted(values: list[int], value: int, expected: list[int]) -> None:
    assert insert_sorted(values, value) == expected


def test_insert_sorted_does_not_modify_the_input() -> None:
    values = [1, 3]

    insert_sorted(values, 2)

    assert values == [1, 3]


def test_pop_at_removes_and_reports() -> None:
    values = [10, 20, 30]

    removed, rest = pop_at(values, 1)

    assert removed == 20
    assert rest == [10, 30]
    assert rest is values


def test_pop_at_supports_negative_indices() -> None:
    removed, rest = pop_at([1, 2, 3], -1)
    assert removed == 3
    assert rest == [1, 2]


def test_pop_at_out_of_range_raises() -> None:
    with pytest.raises(IndexError):
        pop_at([1], 5)


def test_remove_first_removes_only_the_first_match() -> None:
    values = [1, 2, 1, 3]

    assert remove_first(values, 1) is True
    assert values == [2, 1, 3]


def test_remove_first_returns_false_when_absent() -> None:
    values = [1, 2]

    assert remove_first(values, 9) is False
    assert values == [1, 2]


def test_sort_in_place_desc() -> None:
    values = [3, 1, 2]

    assert sort_in_place_desc(values) is None
    assert values == [3, 2, 1]


@pytest.mark.parametrize(
    "nested, expected",
    [
        ([[1, 2], [], [3]], [1, 2, 3]),
        ([], []),
        ([[]], []),
        ([[1], [2], [3]], [1, 2, 3]),
    ],
)
def test_flatten_once(nested: list[list[int]], expected: list[int]) -> None:
    assert flatten_once(nested) == expected
