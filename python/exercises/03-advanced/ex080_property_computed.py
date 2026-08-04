"""Exercise 080 — properties, computed values and caching (advanced).

Goal:   Turn plain attribute access into managed access — validated on write, derived
        on read, and cached when the derivation is expensive.
Drills: `property` getters/setters, read-only computed attributes, a property whose
        setter writes *back* through a conversion, `functools.cached_property`, and
        invalidating a cache by deleting the attribute.
Passes: when `pytest exercises/03-advanced/test_ex080_property_computed.py` is green.

Note:   `property` is a descriptor too (see exercise 079) — the difference is scope. A
        `property` manages *one* attribute of *one* class and can therefore hold the
        conversion logic inline; a descriptor class is what you write when the same
        rule has to be reused. `cached_property` is a *non-data* descriptor: it has no
        `__set__`, so after the first read the value in the instance `__dict__` shadows
        it and the function never runs again.
"""

from typing import Any, Callable


class Rectangle:
    """A rectangle whose sides are validated and whose area is derived."""

    def __init__(self, width: float, height: float) -> None:
        """Assign through the properties so the constructor validates too."""
        raise NotImplementedError

    @property
    def width(self) -> float:
        raise NotImplementedError

    @width.setter
    def width(self, value: float) -> None:
        """A non-number raises TypeError; zero or negative raises ValueError."""
        raise NotImplementedError

    @property
    def height(self) -> float:
        raise NotImplementedError

    @height.setter
    def height(self, value: float) -> None:
        """Same rules as `width`."""
        raise NotImplementedError

    @property
    def area(self) -> float:
        """Read-only: computed on every access, so it can never go stale.

        Assigning to it must raise AttributeError — which a property with no setter
        gives you for free.
        """
        raise NotImplementedError

    @property
    def is_square(self) -> bool:
        raise NotImplementedError


class Temperature:
    """One stored value, two units.

    `celsius` is the state; `fahrenheit` and `kelvin` are views onto it. Assigning to a
    view converts back, so the two can never disagree.
    """

    def __init__(self, celsius: float = 0.0) -> None:
        raise NotImplementedError

    @property
    def celsius(self) -> float:
        raise NotImplementedError

    @celsius.setter
    def celsius(self, value: float) -> None:
        """Below absolute zero (-273.15 °C) raises ValueError."""
        raise NotImplementedError

    @property
    def fahrenheit(self) -> float:
        """``celsius * 9 / 5 + 32``."""
        raise NotImplementedError

    @fahrenheit.setter
    def fahrenheit(self, value: float) -> None:
        """Convert back and store, reusing the `celsius` setter's validation."""
        raise NotImplementedError

    @property
    def kelvin(self) -> float:
        """``celsius + 273.15``, read-only."""
        raise NotImplementedError


class Report:
    """Demonstrates `cached_property` and its invalidation.

    `total` is computed once per instance; `_computations` counts how often the body
    actually ran, which is how the tests can see the cache working.
    """

    def __init__(self, values: list[int]) -> None:
        """Store the values and initialise the computation counter to zero."""
        raise NotImplementedError

    def total(self) -> int:
        """The sum of the values — decorate this with `functools.cached_property`.

        Increment `self._computations` each time the body runs.
        """
        raise NotImplementedError

    def invalidate(self) -> None:
        """Drop the cached `total` so the next read recomputes it.

        `cached_property` stores the value under its own name in the instance dict, so
        deleting that entry is the whole trick. Doing it when nothing is cached must be
        a no-op, not a KeyError.
        """
        raise NotImplementedError


def lazy_attribute(compute: Callable[[Any], Any]) -> property:
    """Build a read-only property that computes once and remembers, using a property.

    The hand-rolled version of `cached_property`: on first read call `compute(instance)`
    and stash the result under ``"_lazy_" + compute.__name__`` in the instance dict; on
    later reads return the stash. Useful for seeing what the decorator does for you.
    """
    raise NotImplementedError
