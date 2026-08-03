from dataclasses import dataclass

import pytest

from ex050_typing_protocol import (
    close_all,
    is_named,
    largest_area,
    names_of,
    total_length,
)


@dataclass
class Person:
    name: str


@dataclass
class Product:
    name: str
    price: int


class Handle:
    def __init__(self) -> None:
        self.closed = False

    def close(self) -> None:
        self.closed = True


class NotCloseable:
    pass


@dataclass
class Circle:
    radius: float

    @property
    def area(self) -> float:
        return 3.14159 * self.radius**2


@dataclass
class Square:
    side: float

    @property
    def area(self) -> float:
        return self.side**2


def test_names_of_accepts_unrelated_classes() -> None:
    # Person and Product share no base class, only the attribute.
    assert names_of([Person("ada"), Product("book", 10)]) == ["ada", "book"]


def test_names_of_empty() -> None:
    assert names_of([]) == []


def test_close_all_closes_what_it_can() -> None:
    a, b = Handle(), Handle()
    other = NotCloseable()

    assert close_all([a, other, b]) == 2
    assert a.closed is True
    assert b.closed is True


def test_close_all_with_nothing_closeable() -> None:
    assert close_all([NotCloseable(), 42, "text"]) == 0


def test_close_all_empty() -> None:
    assert close_all([]) == 0


def test_total_length() -> None:
    assert total_length(["abc", [1, 2], {}]) == 5


def test_total_length_empty() -> None:
    assert total_length([]) == 0


def test_total_length_raises_for_something_without_len() -> None:
    with pytest.raises(TypeError):
        total_length([42])  # type: ignore[list-item]


def test_largest_area() -> None:
    assert largest_area([Square(2.0), Circle(1.0)]) == pytest.approx(4.0)


def test_largest_area_picks_the_circle_when_bigger() -> None:
    assert largest_area([Square(1.0), Circle(2.0)]) == pytest.approx(12.56636)


def test_largest_area_of_no_shapes() -> None:
    assert largest_area([]) == 0.0


def test_is_named_accepts_anything_with_a_name() -> None:
    assert is_named(Person("ada")) is True
    assert is_named(Product("book", 1)) is True


def test_is_named_rejects_something_without_a_name() -> None:
    assert is_named(42) is False
    assert is_named(NotCloseable()) is False
