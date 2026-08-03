import dataclasses

import pytest

from ex045_dataclass_basics import Basket, Point, Temperature, User, to_dict, with_changes


def test_point_is_a_dataclass() -> None:
    assert dataclasses.is_dataclass(Point)


def test_point_constructs_with_a_default_y() -> None:
    assert Point(1).y == 0
    assert Point(1, 2).x == 1


def test_point_equality_is_by_value() -> None:
    assert Point(1, 2) == Point(1, 2)
    assert Point(1, 2) != Point(2, 1)


def test_point_repr() -> None:
    assert repr(Point(1, 2)) == "Point(x=1, y=2)"


def test_basket_default_is_empty() -> None:
    assert Basket().items == []


def test_basket_instances_do_not_share_their_list() -> None:
    first = Basket()
    second = Basket()

    first.add("apple")

    # A shared mutable default would make second.items read ["apple"] too.
    assert first.items == ["apple"]
    assert second.items == []


def test_basket_add_returns_self_for_chaining() -> None:
    basket = Basket()

    result = basket.add("a").add("b")

    assert result is basket
    assert basket.items == ["a", "b"]


def test_basket_accepts_an_explicit_list() -> None:
    assert Basket(["given"]).items == ["given"]


def test_temperature_accepts_a_valid_value() -> None:
    assert Temperature(20.0).celsius == 20.0


def test_temperature_accepts_absolute_zero() -> None:
    assert Temperature(-273.15).celsius == -273.15


@pytest.mark.parametrize("celsius", [-273.16, -300.0, -1000.0])
def test_temperature_rejects_below_absolute_zero(celsius: float) -> None:
    with pytest.raises(ValueError):
        Temperature(celsius)


@pytest.mark.parametrize(
    "celsius, expected",
    [(0.0, 32.0), (100.0, 212.0), (-40.0, -40.0), (37.0, 98.6)],
)
def test_temperature_fahrenheit(celsius: float, expected: float) -> None:
    assert Temperature(celsius).fahrenheit == pytest.approx(expected)


def test_user_derives_the_slug() -> None:
    user = User("Ada Lovelace", "hunter2")

    assert user.slug == "ada-lovelace"


def test_user_slug_is_not_an_init_parameter() -> None:
    # Asserting on the field metadata rather than on a TypeError: the undecorated
    # stub raises TypeError for *any* argument list, so that could never distinguish
    # an implemented class from an unimplemented one.
    slug = next(f for f in dataclasses.fields(User) if f.name == "slug")

    assert slug.init is False


def test_user_repr_hides_the_secret() -> None:
    text = repr(User("Ada", "hunter2"))

    assert "Ada" in text
    assert "hunter2" not in text


def test_to_dict() -> None:
    assert to_dict(Point(1, 2)) == {"x": 1, "y": 2}


def test_to_dict_is_nested() -> None:
    assert to_dict(Basket(["a"])) == {"items": ["a"]}


def test_with_changes_returns_a_new_instance() -> None:
    original = Point(1, 2)

    changed = with_changes(original, y=9)

    assert changed == Point(1, 9)
    assert original == Point(1, 2)
    assert changed is not original


def test_with_changes_revalidates() -> None:
    warm = Temperature(20.0)

    with pytest.raises(ValueError):
        with_changes(warm, celsius=-500.0)


def test_with_changes_without_changes_copies() -> None:
    original = Point(3, 4)
    copy = with_changes(original)

    assert copy == original
    assert copy is not original
