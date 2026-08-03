from enum import Flag, IntFlag

import pytest

from ex048_enum_flag import (
    FileMode,
    Permission,
    add,
    combine,
    from_names,
    has_all,
    has_any,
    remove,
    to_names,
)


def test_permission_is_a_flag() -> None:
    assert issubclass(Permission, Flag)


def test_permission_values_are_powers_of_two() -> None:
    assert (Permission.READ.value, Permission.WRITE.value, Permission.EXECUTE.value) == (1, 2, 4)


def test_permission_combines() -> None:
    combined = Permission.READ | Permission.WRITE

    assert combined.value == 3


def test_file_mode_is_an_int_flag() -> None:
    assert issubclass(FileMode, IntFlag)


def test_file_mode_is_usable_as_an_int() -> None:
    assert (FileMode.APPEND | FileMode.BINARY) == 3


def test_combine_several() -> None:
    assert combine(Permission.READ, Permission.EXECUTE) == Permission.READ | Permission.EXECUTE


def test_combine_one() -> None:
    assert combine(Permission.WRITE) is Permission.WRITE


def test_combine_nothing_is_the_empty_flag() -> None:
    empty = combine()

    assert isinstance(empty, Permission)
    assert empty.value == 0


def test_combine_is_idempotent() -> None:
    assert combine(Permission.READ, Permission.READ) is Permission.READ


def test_has_all() -> None:
    value = Permission.READ | Permission.WRITE

    assert has_all(value, Permission.READ) is True
    assert has_all(value, Permission.READ | Permission.WRITE) is True
    assert has_all(value, Permission.EXECUTE) is False
    assert has_all(value, Permission.READ | Permission.EXECUTE) is False


def test_has_all_of_the_empty_flag_is_true() -> None:
    assert has_all(Permission.READ, combine()) is True


def test_has_any() -> None:
    value = Permission.READ

    assert has_any(value, Permission.READ | Permission.EXECUTE) is True
    assert has_any(value, Permission.WRITE | Permission.EXECUTE) is False


def test_has_any_of_the_empty_flag_is_false() -> None:
    assert has_any(Permission.READ, combine()) is False


def test_add_sets_a_flag() -> None:
    assert add(Permission.READ, Permission.WRITE) == Permission.READ | Permission.WRITE


def test_add_is_idempotent() -> None:
    assert add(Permission.READ, Permission.READ) is Permission.READ


def test_remove_clears_a_flag() -> None:
    value = Permission.READ | Permission.WRITE

    assert remove(value, Permission.WRITE) is Permission.READ


def test_remove_an_unset_flag_changes_nothing() -> None:
    assert remove(Permission.READ, Permission.EXECUTE) is Permission.READ


def test_remove_everything_yields_the_empty_flag() -> None:
    assert remove(Permission.READ, Permission.READ).value == 0


def test_to_names_in_declaration_order() -> None:
    value = Permission.EXECUTE | Permission.READ

    assert to_names(value) == ["READ", "EXECUTE"]


def test_to_names_single() -> None:
    assert to_names(Permission.WRITE) == ["WRITE"]


def test_to_names_empty() -> None:
    assert to_names(combine()) == []


def test_from_names() -> None:
    assert from_names(["READ", "EXECUTE"]) == Permission.READ | Permission.EXECUTE


def test_from_names_empty() -> None:
    assert from_names([]).value == 0


def test_from_names_unknown_raises() -> None:
    with pytest.raises(KeyError):
        from_names(["FLY"])
