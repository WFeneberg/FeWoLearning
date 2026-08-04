from typing import Any

import pytest

from ex079_descriptor_validation import (
    NonEmptyString,
    OneOf,
    Positive,
    Validated,
    storage_keys,
)


def build_product_class() -> type:
    """Create the owner class inside a test body.

    Class creation is what triggers `__set_name__`, so this must not run at import
    time — otherwise an unfinished stub would break collection instead of failing.
    """

    class Product:
        name = NonEmptyString()
        price = Positive()
        category = OneOf("tools", "toys", "books")

        def __init__(self, name: str, price: float, category: str) -> None:
            self.name = name
            self.price = price
            self.category = category

    return Product


def make_product(name: str = "Hammer", price: float = 9.5, category: str = "tools") -> Any:
    return build_product_class()(name, price, category)


def test_valid_values_round_trip() -> None:
    product = make_product()

    assert product.name == "Hammer"
    assert product.price == 9.5
    assert product.category == "tools"


def test_strings_are_stored_stripped() -> None:
    assert make_product(name="  Hammer  ").name == "Hammer"


def test_class_access_returns_the_descriptor() -> None:
    product_class = build_product_class()

    assert isinstance(product_class.price, Positive)
    assert isinstance(product_class.name, NonEmptyString)


def test_set_name_records_both_names() -> None:
    descriptor = build_product_class().price

    assert descriptor.public_name == "price"
    assert descriptor.private_name == "_price"


def test_values_live_in_the_instance_dict() -> None:
    assert storage_keys(make_product()) == ["_category", "_name", "_price"]


def test_instances_do_not_share_state() -> None:
    product_class = build_product_class()
    first = product_class("Hammer", 9.5, "tools")
    second = product_class("Kite", 4.0, "toys")

    # Storing on the descriptor instead of the instance shows up right here.
    assert first.name == "Hammer"
    assert first.price == 9.5
    assert second.name == "Kite"
    assert second.price == 4.0


def test_unset_attribute_raises_attribute_error() -> None:
    class Bare:
        price = Positive()

    with pytest.raises(AttributeError, match="price"):
        Bare().price


@pytest.mark.parametrize("value", [0, -1, -0.5, 0.0])
def test_positive_rejects_non_positive_numbers(value: float) -> None:
    with pytest.raises(ValueError, match="price"):
        make_product(price=value)


@pytest.mark.parametrize("value", ["9.5", None, [1], True])
def test_positive_rejects_non_numbers(value: object) -> None:
    with pytest.raises(TypeError, match="price"):
        make_product(price=value)  # type: ignore[arg-type]


def test_positive_accepts_ints_and_floats() -> None:
    assert make_product(price=1).price == 1
    assert make_product(price=0.01).price == 0.01


@pytest.mark.parametrize("value", ["", "   ", "\t\n"])
def test_non_empty_string_rejects_blanks(value: str) -> None:
    with pytest.raises(ValueError, match="name"):
        make_product(name=value)


@pytest.mark.parametrize("value", [None, 42, ["Hammer"]])
def test_non_empty_string_rejects_non_strings(value: object) -> None:
    with pytest.raises(TypeError, match="name"):
        make_product(name=value)  # type: ignore[arg-type]


def test_one_of_rejects_an_unknown_option() -> None:
    with pytest.raises(ValueError) as info:
        make_product(category="furniture")

    message = str(info.value)
    assert "category" in message
    assert "tools" in message and "toys" in message and "books" in message


def test_one_of_accepts_every_option() -> None:
    for option in ("tools", "toys", "books"):
        assert make_product(category=option).category == option


def test_one_of_requires_at_least_one_option() -> None:
    with pytest.raises(ValueError):
        OneOf()


def test_descriptors_are_reusable_across_owners() -> None:
    class Order:
        quantity = Positive()
        reference = NonEmptyString()

        def __init__(self, quantity: int, reference: str) -> None:
            self.quantity = quantity
            self.reference = reference

    order = Order(3, "ORD-1")

    assert (order.quantity, order.reference) == (3, "ORD-1")
    with pytest.raises(ValueError, match="quantity"):
        Order(0, "ORD-2")


def test_a_custom_subclass_can_transform_the_stored_value() -> None:
    # The base class supplies all the descriptor plumbing; `validate` is the only hook.
    class Upper(Validated):
        def validate(self, value: object) -> str:
            if not isinstance(value, str):
                raise TypeError(f"{self.public_name} must be a string")
            return value.upper()

    class Tag:
        label = Upper()

        def __init__(self, label: str) -> None:
            self.label = label

    assert Tag("abc").label == "ABC"
    with pytest.raises(TypeError, match="label"):
        Tag(1)  # type: ignore[arg-type]


def test_assignment_after_construction_is_validated_too() -> None:
    product = make_product()

    with pytest.raises(ValueError, match="price"):
        product.price = -1

    # The rejected assignment must not have clobbered the old value.
    assert product.price == 9.5
