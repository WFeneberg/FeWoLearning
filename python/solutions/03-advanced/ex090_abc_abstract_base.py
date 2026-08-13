"""Exercise 090 — abc.ABC, abstractmethod and interface checks (reference solution)."""

import math
from abc import ABC, abstractmethod


class Shape(ABC):
    @abstractmethod
    def area(self) -> float:
        raise NotImplementedError

    @abstractmethod
    def perimeter(self) -> float:
        raise NotImplementedError

    def describe(self) -> str:
        return f"{type(self).__name__}: area={self.area():.2f}, perimeter={self.perimeter():.2f}"


class Rectangle(Shape):
    def __init__(self, width: float, height: float) -> None:
        self.width = width
        self.height = height

    def area(self) -> float:
        return self.width * self.height

    def perimeter(self) -> float:
        return 2 * (self.width + self.height)


class Circle(Shape):
    def __init__(self, radius: float) -> None:
        self.radius = radius

    def area(self) -> float:
        return math.pi * self.radius**2

    def perimeter(self) -> float:
        return 2 * math.pi * self.radius


class IncompleteShape(Shape):
    def area(self) -> float:
        return 0.0


class Square:
    def __init__(self, side: float) -> None:
        self.side = side

    def area(self) -> float:
        return self.side**2

    def perimeter(self) -> float:
        return 4 * self.side


def register_virtual_shape(cls: type) -> None:
    Shape.register(cls)
