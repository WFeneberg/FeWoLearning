from typing import Any

import pytest

from ex051_typed_dict import (
    Partial,
    UserRecord,
    city_of,
    display_name,
    is_valid_user,
    make_user,
    merge_partial,
    required_keys,
)


def test_make_user_without_an_email_omits_the_key() -> None:
    user = make_user(1, "ada")

    assert user == {"id": 1, "name": "ada"}
    assert "email" not in user


def test_make_user_with_an_email() -> None:
    assert make_user(1, "ada", "ada@example.com") == {
        "id": 1,
        "name": "ada",
        "email": "ada@example.com",
    }


def test_make_user_returns_a_plain_dict() -> None:
    # A TypedDict has no runtime identity of its own.
    assert type(make_user(1, "ada")) is dict


def test_is_valid_user_accepts_a_minimal_record() -> None:
    assert is_valid_user({"id": 1, "name": "ada"}) is True


def test_is_valid_user_accepts_the_optional_keys() -> None:
    value: Any = {
        "id": 1,
        "name": "ada",
        "email": "a@b.c",
        "address": {"street": "Main", "city": "Bern"},
    }
    assert is_valid_user(value) is True


@pytest.mark.parametrize(
    "value",
    [
        {"id": 1},                                  # missing name
        {"name": "ada"},                            # missing id
        {"id": "1", "name": "ada"},                 # id is not an int
        {"id": 1, "name": 2},                       # name is not a str
        {"id": 1, "name": "ada", "email": 5},       # email is not a str
        {"id": 1, "name": "ada", "extra": True},    # unknown key
        {},
        "not a dict",
        None,
    ],
)
def test_is_valid_user_rejects_bad_payloads(value: Any) -> None:
    assert is_valid_user(value) is False


def test_display_name_with_an_email() -> None:
    user: UserRecord = {"id": 1, "name": "ada", "email": "ada@example.com"}

    assert display_name(user) == "ada <ada@example.com>"


def test_display_name_without_an_email() -> None:
    user: UserRecord = {"id": 1, "name": "ada"}

    assert display_name(user) == "ada"


def test_city_of() -> None:
    user: UserRecord = {
        "id": 1,
        "name": "ada",
        "address": {"street": "Main", "city": "Bern"},
    }

    assert city_of(user) == "Bern"


def test_city_of_without_an_address() -> None:
    user: UserRecord = {"id": 1, "name": "ada"}

    assert city_of(user) == "unknown"
    assert city_of(user, "n/a") == "n/a"


def test_merge_partial() -> None:
    base: Partial = {"a": 1}
    override: Partial = {"b": "x"}

    assert merge_partial(base, override) == {"a": 1, "b": "x"}


def test_merge_partial_override_wins() -> None:
    assert merge_partial({"a": 1}, {"a": 2}) == {"a": 2}


def test_merge_partial_does_not_modify_its_inputs() -> None:
    base: Partial = {"a": 1}
    override: Partial = {"a": 2}

    merge_partial(base, override)

    assert base == {"a": 1}
    assert override == {"a": 2}


def test_merge_partial_with_empty_sides() -> None:
    assert merge_partial({}, {}) == {}


def test_required_keys() -> None:
    assert required_keys() == {"id", "name"}
