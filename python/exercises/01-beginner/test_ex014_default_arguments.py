import pytest

from ex014_default_arguments import (
    append_item,
    build_config,
    counter_factory,
    greet,
    repeat,
    slice_window,
)


def test_greet_uses_the_default_greeting() -> None:
    assert greet("Ada") == "Hello, Ada!"


def test_greet_with_a_custom_greeting() -> None:
    assert greet("Ada", "Hi") == "Hi, Ada!"


def test_append_item_creates_a_list_when_none_is_given() -> None:
    assert append_item(1) == [1]


def test_append_item_does_not_share_state_between_calls() -> None:
    first = append_item(1)
    second = append_item(2)

    # The mutable-default trap would make this [1, 2] both times.
    assert first == [1]
    assert second == [2]
    assert first is not second


def test_append_item_uses_the_given_list() -> None:
    target = [1]

    result = append_item(2, target)

    assert result == [1, 2]
    assert result is target


def test_build_config_defaults() -> None:
    assert build_config() == {"host": "localhost", "port": "8080"}


def test_build_config_applies_overrides() -> None:
    assert build_config({"port": "9000"}) == {"host": "localhost", "port": "9000"}


def test_build_config_can_add_keys() -> None:
    assert build_config({"debug": "1"}) == {
        "host": "localhost",
        "port": "8080",
        "debug": "1",
    }


def test_build_config_does_not_modify_the_caller_dict() -> None:
    overrides = {"port": "9000"}

    build_config(overrides)

    assert overrides == {"port": "9000"}


def test_build_config_returns_independent_dicts() -> None:
    first = build_config()
    first["host"] = "changed"

    assert build_config()["host"] == "localhost"


@pytest.mark.parametrize(
    "text, times, separator, expected",
    [
        ("ab", 2, " ", "ab ab"),
        ("ab", 3, "-", "ab-ab-ab"),
        ("ab", 1, " ", "ab"),
        ("ab", 0, " ", ""),
        ("ab", -1, " ", ""),
    ],
)
def test_repeat(text: str, times: int, separator: str, expected: str) -> None:
    assert repeat(text, times, separator) == expected


def test_repeat_defaults() -> None:
    assert repeat("x") == "x x"


@pytest.mark.parametrize(
    "values, start, length, expected",
    [
        ([1, 2, 3, 4], 1, 2, [2, 3]),
        ([1, 2, 3, 4], 2, None, [3, 4]),
        ([1, 2, 3, 4], 0, None, [1, 2, 3, 4]),
        ([1, 2, 3], 0, 10, [1, 2, 3]),
        ([1, 2, 3], 5, 2, []),
        ([], 0, None, []),
    ],
)
def test_slice_window(
    values: list[int], start: int, length: int | None, expected: list[int]
) -> None:
    assert slice_window(values, start, length) == expected


def test_slice_window_defaults_to_the_whole_list() -> None:
    assert slice_window([1, 2]) == [1, 2]


def test_counter_factory_returns_a_new_list_each_time() -> None:
    first_list, first_start = counter_factory()
    second_list, _ = counter_factory(5)

    assert first_start == 0
    assert first_list is not second_list

    first_list.append(1)
    assert counter_factory()[0] == []
