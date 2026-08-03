from typing import Any

import pytest

from ex031_nested_data_access import (
    collect_values,
    count_leaves,
    deep_merge,
    flatten_keys,
    get_path,
    set_path,
)

DATA: dict[str, Any] = {
    "users": [
        {"name": "ada", "tags": ["math", "code"]},
        {"name": "grace", "tags": []},
    ],
    "meta": {"version": 2, "nested": {"deep": "value"}},
}


@pytest.mark.parametrize(
    "path, expected",
    [
        ("meta.version", 2),
        ("meta.nested.deep", "value"),
        ("users.0.name", "ada"),
        ("users.1.name", "grace"),
        ("users.0.tags.1", "code"),
        ("", DATA),
    ],
)
def test_get_path_finds_values(path: str, expected: Any) -> None:
    assert get_path(DATA, path) == expected


@pytest.mark.parametrize(
    "path",
    [
        "meta.missing",
        "nope",
        "users.9.name",
        "meta.version.deeper",
        "users.notanindex",
        "users.0.tags.9",
    ],
)
def test_get_path_returns_the_default_when_the_walk_breaks(path: str) -> None:
    assert get_path(DATA, path) is None
    assert get_path(DATA, path, "fallback") == "fallback"


def test_set_path_writes_an_existing_branch() -> None:
    data: dict[str, Any] = {"a": {"b": 1}}

    result = set_path(data, "a.b", 2)

    assert result == {"a": {"b": 2}}
    assert result is data


def test_set_path_creates_intermediate_dicts() -> None:
    data: dict[str, Any] = {}

    set_path(data, "a.b.c", "deep")

    assert data == {"a": {"b": {"c": "deep"}}}


def test_set_path_keeps_siblings() -> None:
    data: dict[str, Any] = {"a": {"keep": 1}}

    set_path(data, "a.new", 2)

    assert data == {"a": {"keep": 1, "new": 2}}


def test_set_path_single_segment() -> None:
    data: dict[str, Any] = {}
    set_path(data, "top", 1)
    assert data == {"top": 1}


def test_set_path_empty_path_raises() -> None:
    with pytest.raises(ValueError):
        set_path({}, "", 1)


def test_flatten_keys() -> None:
    assert flatten_keys({"a": {"b": 1}, "c": 2}) == {"a.b": 1, "c": 2}


def test_flatten_keys_deeply() -> None:
    assert flatten_keys({"a": {"b": {"c": 3}}}) == {"a.b.c": 3}


def test_flatten_keys_treats_lists_as_leaves() -> None:
    assert flatten_keys({"a": [1, 2]}) == {"a": [1, 2]}


def test_flatten_keys_custom_separator() -> None:
    assert flatten_keys({"a": {"b": 1}}, "/") == {"a/b": 1}


def test_flatten_keys_drops_empty_nested_dicts() -> None:
    assert flatten_keys({"a": {}, "b": 1}) == {"b": 1}


def test_flatten_keys_empty() -> None:
    assert flatten_keys({}) == {}


def test_collect_values_across_lists_and_dicts() -> None:
    assert collect_values(DATA, "name") == ["ada", "grace"]


def test_collect_values_finds_a_deep_key() -> None:
    assert collect_values(DATA, "deep") == ["value"]


def test_collect_values_without_matches() -> None:
    assert collect_values(DATA, "absent") == []


def test_collect_values_on_a_scalar() -> None:
    assert collect_values(42, "name") == []


def test_deep_merge_merges_nested_dicts() -> None:
    base = {"a": {"x": 1, "y": 2}}
    override = {"a": {"y": 20, "z": 30}}

    assert deep_merge(base, override) == {"a": {"x": 1, "y": 20, "z": 30}}


def test_deep_merge_replaces_non_dict_values() -> None:
    assert deep_merge({"a": {"x": 1}}, {"a": "scalar"}) == {"a": "scalar"}


def test_deep_merge_adds_new_keys() -> None:
    assert deep_merge({"a": 1}, {"b": 2}) == {"a": 1, "b": 2}


def test_deep_merge_does_not_modify_its_arguments() -> None:
    base = {"a": {"x": 1}}
    override = {"a": {"y": 2}}

    deep_merge(base, override)

    assert base == {"a": {"x": 1}}
    assert override == {"a": {"y": 2}}


def test_deep_merge_with_empty_sides() -> None:
    assert deep_merge({}, {"a": 1}) == {"a": 1}
    assert deep_merge({"a": 1}, {}) == {"a": 1}


@pytest.mark.parametrize(
    "data, expected",
    [
        ({"a": 1, "b": 2}, 2),
        ({"a": {"b": 1}}, 1),
        ({"a": [1, 2, 3]}, 3),
        ({}, 0),
        ({"a": []}, 0),
        (42, 1),
        ({"a": {"b": [1, {"c": 2}]}}, 2),
    ],
)
def test_count_leaves(data: Any, expected: int) -> None:
    assert count_leaves(data) == expected
