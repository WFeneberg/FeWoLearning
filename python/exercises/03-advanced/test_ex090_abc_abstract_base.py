import math

import pytest

from ex090_abc_abstract_base import Circle, IncompleteShape, Rectangle, Shape, Square, register_virtual_shape


def test_shape_cannot_be_instantiated():
    with pytest.raises(TypeError):
        Shape()  # type: ignore[abstract]


def test_incomplete_subclass_cannot_be_instantiated():
    with pytest.raises(TypeError):
        IncompleteShape()  # type: ignore[abstract]


def test_rectangle_area_and_perimeter():
    rectangle = Rectangle(3, 4)
    assert rectangle.area() == 12
    assert rectangle.perimeter() == 14


def test_circle_area_and_perimeter():
    circle = Circle(2)
    assert math.isclose(circle.area(), math.pi * 4)
    assert math.isclose(circle.perimeter(), 2 * math.pi * 2)


def test_describe_combines_area_and_perimeter():
    assert Rectangle(3, 4).describe() == "Rectangle: area=12.00, perimeter=14.00"


def test_register_virtual_shape_makes_isinstance_true_without_inheritance():
    assert not issubclass(Square, Shape)

    register_virtual_shape(Square)

    assert issubclass(Square, Shape)
    assert isinstance(Square(3), Shape)


def test_registering_twice_is_harmless():
    register_virtual_shape(Square)
    register_virtual_shape(Square)

    assert isinstance(Square(1), Shape)
