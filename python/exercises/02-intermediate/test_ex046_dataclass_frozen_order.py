import dataclasses

import pytest

from ex046_dataclass_frozen_order import (
    Coord,
    Money,
    Priority,
    Version,
    sort_versions,
    unique_amounts,
)


def test_money_constructs() -> None:
    money = Money(500, "EUR")

    assert money.amount == 500
    assert money.currency == "EUR"


def test_money_is_frozen() -> None:
    money = Money(500, "EUR")

    with pytest.raises(dataclasses.FrozenInstanceError):
        money.amount = 600  # type: ignore[misc]


def test_money_is_hashable() -> None:
    assert len({Money(1, "EUR"), Money(1, "EUR"), Money(2, "EUR")}) == 2


def test_money_works_as_a_dict_key() -> None:
    prices = {Money(100, "EUR"): "cheap"}

    assert prices[Money(100, "EUR")] == "cheap"


def test_money_plus() -> None:
    total = Money(100, "EUR").plus(Money(50, "EUR"))

    assert total == Money(150, "EUR")


def test_money_plus_returns_a_new_instance() -> None:
    original = Money(100, "EUR")

    total = original.plus(Money(1, "EUR"))

    assert original == Money(100, "EUR")
    assert total is not original


def test_money_plus_rejects_mixed_currencies() -> None:
    with pytest.raises(ValueError):
        Money(100, "EUR").plus(Money(100, "CHF"))


def test_version_orders_by_major_then_minor_then_patch() -> None:
    assert Version(1, 0, 0) < Version(1, 0, 1)
    assert Version(1, 0, 9) < Version(1, 1, 0)
    assert Version(1, 9, 9) < Version(2, 0, 0)


def test_version_supports_all_comparisons() -> None:
    a, b = Version(1, 2, 3), Version(1, 2, 4)

    assert a <= b
    assert b > a
    assert b >= a
    assert a != b


def test_version_equality() -> None:
    assert Version(1, 2, 3) == Version(1, 2, 3)


def test_priority_ignores_the_note_for_equality() -> None:
    assert Priority(1, "first note") == Priority(1, "different note")


def test_priority_ignores_the_note_for_ordering() -> None:
    assert Priority(1, "zzz") < Priority(2, "aaa")


def test_priority_orders_by_rank() -> None:
    items = [Priority(3, "c"), Priority(1, "a"), Priority(2, "b")]

    assert [p.rank for p in sorted(items)] == [1, 2, 3]


def test_coord_constructs_and_reads() -> None:
    coord = Coord(1, 2)

    assert (coord.x, coord.y) == (1, 2)


def test_coord_is_frozen() -> None:
    with pytest.raises(dataclasses.FrozenInstanceError):
        Coord(1, 2).x = 3  # type: ignore[misc]


def test_coord_rejects_unknown_attributes() -> None:
    coord = Coord(1, 2)

    # With slots there is no __dict__, so a typo cannot silently stick.
    with pytest.raises(AttributeError):
        coord.z = 3  # type: ignore[attr-defined]


def test_coord_has_no_instance_dict() -> None:
    assert not hasattr(Coord(1, 2), "__dict__")


def test_sort_versions() -> None:
    versions = [Version(2, 0, 0), Version(1, 5, 0), Version(1, 5, 1)]

    assert sort_versions(versions) == [Version(1, 5, 0), Version(1, 5, 1), Version(2, 0, 0)]


def test_sort_versions_empty() -> None:
    assert sort_versions([]) == []


def test_unique_amounts() -> None:
    values = [Money(1, "EUR"), Money(1, "EUR"), Money(2, "EUR"), Money(1, "CHF")]

    assert unique_amounts(values) == 3


def test_unique_amounts_empty() -> None:
    assert unique_amounts([]) == 0
