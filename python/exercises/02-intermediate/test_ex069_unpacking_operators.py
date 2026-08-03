from typing import Any

import pytest

from ex069_unpacking_operators import (
    call_with_dict,
    call_with_list,
    concat_lists,
    forward_all,
    merge,
    merge_with_extra,
    split_ends,
    split_first_rest,
    to_set_literal,
)


def test_call_with_list() -> None:
    assert call_with_list(max, [1, 5, 3]) == 5


def test_call_with_list_single_argument() -> None:
    assert call_with_list(abs, [-4]) == 4


def test_call_with_dict() -> None:
    def connect(host: str, port: int) -> str:
        return f"{host}:{port}"

    assert call_with_dict(connect, {"host": "db", "port": 5432}) == "db:5432"


def test_call_with_dict_empty() -> None:
    assert call_with_dict(dict, {}) == {}


def test_concat_lists() -> None:
    assert concat_lists([1, 2], [3], []) == [1, 2, 3]


def test_concat_lists_with_nothing() -> None:
    assert concat_lists() == []


def test_concat_lists_accepts_other_iterables() -> None:
    assert concat_lists([1], (2, 3)) == [1, 2, 3]  # type: ignore[arg-type]


def test_merge() -> None:
    assert merge({"a": 1}, {"b": 2}) == {"a": 1, "b": 2}


def test_merge_later_wins() -> None:
    assert merge({"a": 1}, {"a": 2}, {"a": 3}) == {"a": 3}


def test_merge_does_not_modify_its_inputs() -> None:
    first = {"a": 1}
    second = {"a": 2}

    merge(first, second)

    assert first == {"a": 1}
    assert second == {"a": 2}


def test_merge_with_nothing() -> None:
    assert merge() == {}


def test_merge_with_extra_keywords_win() -> None:
    assert merge_with_extra({"a": 1, "b": 2}, b=99) == {"a": 1, "b": 99}


def test_merge_with_extra_without_keywords() -> None:
    assert merge_with_extra({"a": 1}) == {"a": 1}


def test_merge_with_extra_does_not_modify_the_base() -> None:
    base = {"a": 1}

    merge_with_extra(base, a=2)

    assert base == {"a": 1}


@pytest.mark.parametrize(
    "values, expected",
    [([1, 2, 3], (1, [2, 3])), ([7], (7, [])), ((1, 2), (1, [2]))],
)
def test_split_first_rest(values: Any, expected: tuple[Any, list[Any]]) -> None:
    assert split_first_rest(values) == expected


def test_split_first_rest_of_a_generator() -> None:
    assert split_first_rest(n for n in [4, 5, 6]) == (4, [5, 6])


def test_split_first_rest_empty_raises() -> None:
    with pytest.raises(ValueError):
        split_first_rest([])


@pytest.mark.parametrize(
    "values, expected",
    [
        ([1, 2, 3, 4], (1, [2, 3], 4)),
        ([1, 2, 3], (1, [2], 3)),
        ([1, 2], (1, [], 2)),
    ],
)
def test_split_ends(values: list[Any], expected: tuple[Any, list[Any], Any]) -> None:
    assert split_ends(values) == expected


@pytest.mark.parametrize("values", [[], [1]])
def test_split_ends_needs_two_items(values: list[Any]) -> None:
    with pytest.raises(ValueError):
        split_ends(values)


def test_to_set_literal() -> None:
    assert to_set_literal([1, 2], [2, 3]) == {1, 2, 3}


def test_to_set_literal_with_nothing_is_an_empty_set() -> None:
    result = to_set_literal()

    assert result == set()
    assert isinstance(result, set)


def test_to_set_literal_mixed_iterables() -> None:
    assert to_set_literal("ab", ["b", "c"]) == {"a", "b", "c"}


def test_forward_all() -> None:
    def describe(a: int, b: int, sep: str = "-") -> str:
        return f"{a}{sep}{b}"

    assert forward_all(describe, [1, 2], {"sep": "+"}) == "1+2"


def test_forward_all_without_keywords() -> None:
    assert forward_all(max, [3, 9], {}) == 9
