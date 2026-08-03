from enum import Enum, IntEnum, StrEnum

import pytest

from ex047_enum_basics import (
    Color,
    Level,
    Status,
    Suffix,
    all_names,
    by_name,
    by_value,
    levels_at_least,
    parse_status,
)


def test_color_is_an_enum() -> None:
    assert issubclass(Color, Enum)


def test_color_auto_values_count_from_one() -> None:
    assert (Color.RED.value, Color.GREEN.value, Color.BLUE.value) == (1, 2, 3)


def test_color_iteration_is_in_declaration_order() -> None:
    assert [c.name for c in Color] == ["RED", "GREEN", "BLUE"]


def test_color_members_are_singletons() -> None:
    assert Color.RED is Color(1)


def test_status_values() -> None:
    assert Status.PENDING.value == "pending"
    assert Status.ACTIVE.value == "active"
    assert Status.DONE.value == "done"


def test_status_is_final_only_for_done() -> None:
    assert Status.DONE.is_final is True
    assert Status.PENDING.is_final is False
    assert Status.ACTIVE.is_final is False


def test_level_is_an_int_enum() -> None:
    assert issubclass(Level, IntEnum)


def test_level_compares_with_plain_ints() -> None:
    assert Level.LOW < 3
    assert Level.HIGH == 10
    assert Level.MEDIUM > Level.LOW


def test_level_usable_in_arithmetic() -> None:
    assert Level.LOW + Level.MEDIUM == 6


def test_suffix_is_a_str_enum() -> None:
    assert issubclass(Suffix, StrEnum)


def test_suffix_behaves_like_a_string() -> None:
    assert Suffix.TXT == "txt"
    assert f"file.{Suffix.MD}" == "file.md"


def test_by_value() -> None:
    assert by_value(2) is Color.GREEN


def test_by_value_unknown_raises_value_error() -> None:
    with pytest.raises(ValueError):
        by_value(99)


def test_by_name() -> None:
    assert by_name("BLUE") is Color.BLUE


def test_by_name_unknown_raises_key_error() -> None:
    with pytest.raises(KeyError):
        by_name("PURPLE")


def test_all_names() -> None:
    assert all_names() == ["RED", "GREEN", "BLUE"]


# Parametrising on the member *name* rather than the member itself: a parametrize
# decorator is evaluated at collection time, where an unimplemented stub has no
# members yet — that would be a collection error instead of a failing test.
@pytest.mark.parametrize(
    "text, member_name",
    [("pending", "PENDING"), ("active", "ACTIVE"), ("done", "DONE")],
)
def test_parse_status_known_values(text: str, member_name: str) -> None:
    assert parse_status(text) is getattr(Status, member_name)


def test_parse_status_unknown_returns_the_default() -> None:
    assert parse_status("nope") is None
    assert parse_status("nope", Status.PENDING) is Status.PENDING


def test_levels_at_least() -> None:
    assert levels_at_least(Level.MEDIUM) == [Level.MEDIUM, Level.HIGH]


def test_levels_at_least_lowest_returns_all() -> None:
    assert levels_at_least(Level.LOW) == [Level.LOW, Level.MEDIUM, Level.HIGH]


def test_levels_at_least_highest() -> None:
    assert levels_at_least(Level.HIGH) == [Level.HIGH]
