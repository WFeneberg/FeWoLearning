import pytest

from ex026_sort_multiple_keys import (
    by_age_desc_then_name,
    by_city_then_name,
    by_field_names,
    by_index,
    chained_sort,
    group_sizes,
)

RECORDS: list[tuple[str, int, str]] = [
    ("ada", 36, "London"),
    ("grace", 45, "Arlington"),
    ("alan", 41, "London"),
    ("mia", 36, "Arlington"),
]


def test_by_city_then_name() -> None:
    assert by_city_then_name(RECORDS) == [
        ("grace", 45, "Arlington"),
        ("mia", 36, "Arlington"),
        ("ada", 36, "London"),
        ("alan", 41, "London"),
    ]


def test_by_city_then_name_does_not_modify_the_input() -> None:
    original = list(RECORDS)

    by_city_then_name(RECORDS)

    assert RECORDS == original


def test_by_age_desc_then_name() -> None:
    assert by_age_desc_then_name(RECORDS) == [
        ("grace", 45, "Arlington"),
        ("alan", 41, "London"),
        ("ada", 36, "London"),
        ("mia", 36, "Arlington"),
    ]


def test_by_age_desc_keeps_names_ascending_within_a_tie() -> None:
    result = by_age_desc_then_name(RECORDS)
    tied = [name for name, age, _ in result if age == 36]

    assert tied == ["ada", "mia"]


def test_by_index_first_element() -> None:
    rows = [(3, "c"), (1, "a"), (2, "b")]
    assert by_index(rows, 0) == [(1, "a"), (2, "b"), (3, "c")]


def test_by_index_second_element() -> None:
    rows = [(1, "c"), (2, "a"), (3, "b")]
    assert by_index(rows, 1) == [(2, "a"), (3, "b"), (1, "c")]


def test_by_index_out_of_range_raises() -> None:
    with pytest.raises(IndexError):
        by_index([(1, "a")], 5)


def test_by_index_empty_input() -> None:
    assert by_index([], 0) == []


def test_by_field_names_single_field() -> None:
    rows: list[dict[str, object]] = [{"n": 2}, {"n": 1}]
    assert by_field_names(rows, ["n"]) == [{"n": 1}, {"n": 2}]


def test_by_field_names_multiple_fields() -> None:
    rows: list[dict[str, object]] = [
        {"city": "B", "name": "z"},
        {"city": "A", "name": "y"},
        {"city": "B", "name": "a"},
    ]
    assert by_field_names(rows, ["city", "name"]) == [
        {"city": "A", "name": "y"},
        {"city": "B", "name": "a"},
        {"city": "B", "name": "z"},
    ]


def test_by_field_names_without_fields_keeps_order() -> None:
    rows: list[dict[str, object]] = [{"n": 2}, {"n": 1}]
    assert by_field_names(rows, []) == [{"n": 2}, {"n": 1}]


def test_chained_sort_matches_a_single_tuple_key_sort() -> None:
    # City ascending, age descending.
    assert chained_sort(RECORDS) == [
        ("grace", 45, "Arlington"),
        ("mia", 36, "Arlington"),
        ("alan", 41, "London"),
        ("ada", 36, "London"),
    ]


def test_chained_sort_empty() -> None:
    assert chained_sort([]) == []


def test_group_sizes() -> None:
    assert group_sizes(RECORDS) == [("Arlington", 2), ("London", 2)]


def test_group_sizes_orders_by_count_first() -> None:
    records: list[tuple[str, int, str]] = [
        ("a", 1, "Zurich"),
        ("b", 1, "Bern"),
        ("c", 1, "Bern"),
    ]
    assert group_sizes(records) == [("Bern", 2), ("Zurich", 1)]


def test_group_sizes_empty() -> None:
    assert group_sizes([]) == []
