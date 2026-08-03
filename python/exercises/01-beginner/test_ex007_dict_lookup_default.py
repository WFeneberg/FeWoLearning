import pytest

from ex007_dict_lookup_default import (
    add_to_group,
    increment,
    lookup,
    nested_get,
    require,
    take,
)


def test_lookup_returns_the_value() -> None:
    assert lookup({"ada": 10}, "ada") == 10


def test_lookup_falls_back_to_zero() -> None:
    assert lookup({}, "nobody") == 0


def test_lookup_honours_a_custom_default() -> None:
    assert lookup({}, "nobody", -1) == -1


def test_require_returns_the_value() -> None:
    assert require({"ada": 10}, "ada") == 10


def test_require_raises_for_a_missing_key() -> None:
    with pytest.raises(KeyError):
        require({}, "nobody")


def test_add_to_group_creates_the_list() -> None:
    groups: dict[str, list[str]] = {}

    result = add_to_group(groups, "admins", "ada")

    assert result == {"admins": ["ada"]}
    assert result is groups


def test_add_to_group_appends_to_an_existing_list() -> None:
    groups = {"admins": ["ada"]}

    add_to_group(groups, "admins", "grace")

    assert groups == {"admins": ["ada", "grace"]}


def test_add_to_group_keeps_other_keys() -> None:
    groups = {"admins": ["ada"]}

    add_to_group(groups, "users", "linus")

    assert groups == {"admins": ["ada"], "users": ["linus"]}


def test_increment_starts_from_zero() -> None:
    counts: dict[str, int] = {}

    result = increment(counts, "hits")

    assert result == {"hits": 1}
    assert result is counts


def test_increment_adds_to_an_existing_count() -> None:
    counts = {"hits": 5}
    increment(counts, "hits")
    assert counts == {"hits": 6}


def test_increment_by_a_custom_step() -> None:
    counts = {"hits": 5}
    increment(counts, "hits", 10)
    assert counts == {"hits": 15}


def test_take_removes_and_returns() -> None:
    settings = {"theme": "dark", "lang": "de"}

    value, rest = take(settings, "theme")

    assert value == "dark"
    assert rest == {"lang": "de"}
    assert rest is settings


def test_take_of_a_missing_key_uses_the_default() -> None:
    settings = {"lang": "de"}

    value, rest = take(settings, "theme", "light")

    assert value == "light"
    assert rest == {"lang": "de"}


@pytest.mark.parametrize(
    "data, path, expected",
    [
        ({"a": {"b": 1}}, ["a", "b"], 1),
        ({"a": {"b": {"c": "deep"}}}, ["a", "b", "c"], "deep"),
        ({"a": 1}, [], {"a": 1}),
        ({"a": {"b": 1}}, ["a"], {"b": 1}),
    ],
)
def test_nested_get_finds_values(data: dict[str, object], path: list[str], expected: object) -> None:
    assert nested_get(data, path) == expected


@pytest.mark.parametrize(
    "data, path",
    [
        ({"a": {"b": 1}}, ["a", "missing"]),
        ({"a": {"b": 1}}, ["missing"]),
        # "b" is an int, so the walk cannot continue into it.
        ({"a": {"b": 1}}, ["a", "b", "c"]),
        ({}, ["a"]),
    ],
)
def test_nested_get_returns_none_when_the_walk_breaks(
    data: dict[str, object], path: list[str]
) -> None:
    assert nested_get(data, path) is None


def test_nested_get_honours_a_custom_default() -> None:
    assert nested_get({"a": 1}, ["nope"], "fallback") == "fallback"
