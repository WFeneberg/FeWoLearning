from datetime import date
from decimal import Decimal
from typing import Any

import pytest

from ex061_json_roundtrip import (
    from_json,
    is_json_safe,
    parse_with_dates,
    round_trip,
    to_json,
    to_json_extended,
    to_pretty_json,
)


def test_to_json_sorts_keys_and_omits_spaces() -> None:
    assert to_json({"b": 1, "a": 2}) == '{"a":2,"b":1}'


def test_to_json_nested() -> None:
    assert to_json({"outer": {"z": 1, "a": 2}}) == '{"outer":{"a":2,"z":1}}'


def test_to_json_list() -> None:
    assert to_json([1, "two", None, True]) == '[1,"two",null,true]'


def test_to_pretty_json_indents_by_two() -> None:
    assert to_pretty_json({"a": 1}) == '{\n  "a": 1\n}'


def test_to_pretty_json_sorts_keys() -> None:
    assert to_pretty_json({"b": 1, "a": 2}) == '{\n  "a": 2,\n  "b": 1\n}'


def test_from_json() -> None:
    assert from_json('{"a": 1}') == {"a": 1}


@pytest.mark.parametrize("text", ["{not json", "", "{'single': 'quotes'}"])
def test_from_json_rejects_invalid_input(text: str) -> None:
    with pytest.raises(ValueError):
        from_json(text)


def test_to_json_extended_handles_a_date() -> None:
    assert to_json_extended({"when": date(2024, 8, 3)}) == '{"when":"2024-08-03"}'


def test_to_json_extended_handles_a_decimal_as_a_string() -> None:
    # A float would lose precision, which is the whole reason to use Decimal.
    assert to_json_extended({"price": Decimal("0.1")}) == '{"price":"0.1"}'


def test_to_json_extended_handles_a_set_deterministically() -> None:
    assert to_json_extended({"tags": {"b", "a"}}) == '{"tags":["a","b"]}'


def test_to_json_extended_still_rejects_unknown_types() -> None:
    class Custom:
        pass

    with pytest.raises(TypeError):
        to_json_extended({"x": Custom()})


def test_to_json_extended_leaves_plain_values_alone() -> None:
    assert to_json_extended({"n": 1}) == '{"n":1}'


def test_parse_with_dates_converts_the_listed_keys() -> None:
    result = parse_with_dates('{"when": "2024-08-03", "note": "text"}', {"when"})

    assert result == {"when": date(2024, 8, 3), "note": "text"}


def test_parse_with_dates_works_on_nested_objects() -> None:
    result = parse_with_dates('{"outer": {"when": "2024-01-02"}}', {"when"})

    assert result == {"outer": {"when": date(2024, 1, 2)}}


def test_parse_with_dates_leaves_unlisted_keys_as_strings() -> None:
    result = parse_with_dates('{"when": "2024-08-03"}', set())

    assert result == {"when": "2024-08-03"}


def test_parse_with_dates_inside_a_list() -> None:
    result = parse_with_dates('[{"when": "2024-08-03"}]', {"when"})

    assert result == [{"when": date(2024, 8, 3)}]


def test_round_trip_preserves_plain_data() -> None:
    value: Any = {"a": [1, 2], "b": "text", "c": None}

    assert round_trip(value) == value


def test_round_trip_turns_a_tuple_into_a_list() -> None:
    assert round_trip({"pair": (1, 2)}) == {"pair": [1, 2]}


def test_round_trip_stringifies_non_string_keys() -> None:
    assert round_trip({1: "one"}) == {"1": "one"}


@pytest.mark.parametrize(
    "value, expected",
    [
        ({"a": 1}, True),
        ([1, "two", None], True),
        ({"a": {1, 2}}, False),
        ({"d": date(2024, 1, 1)}, False),
        ({"x": Decimal("1")}, False),
        ("plain string", True),
    ],
)
def test_is_json_safe(value: Any, expected: bool) -> None:
    assert is_json_safe(value) is expected
