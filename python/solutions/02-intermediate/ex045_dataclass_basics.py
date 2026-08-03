"""Exercise 045 — dataclasses (reference solution)."""

import dataclasses
from dataclasses import dataclass, field
from typing import Any

ABSOLUTE_ZERO_C = -273.15


@dataclass
class Point:
    x: int
    y: int = 0


@dataclass
class Basket:
    # default_factory is called per instance. A bare `= []` is rejected by the
    # decorator itself, which is how dataclasses prevent the shared-default bug.
    items: list[str] = field(default_factory=list)

    def add(self, name: str) -> "Basket":
        self.items.append(name)
        return self


@dataclass
class Temperature:
    celsius: float

    def __post_init__(self) -> None:
        # Runs after the generated __init__ has assigned the fields, so validation
        # sees the final values.
        if self.celsius < ABSOLUTE_ZERO_C:
            raise ValueError(f"celsius must not be below {ABSOLUTE_ZERO_C}")

    @property
    def fahrenheit(self) -> float:
        return self.celsius * 9 / 5 + 32


@dataclass
class User:
    name: str
    # repr=False keeps the value out of the generated repr.
    secret: str = field(repr=False)
    # init=False keeps it out of the signature; __post_init__ has to fill it in.
    slug: str = field(init=False)

    def __post_init__(self) -> None:
        self.slug = self.name.lower().replace(" ", "-")


def to_dict(instance: Any) -> dict[str, Any]:
    return dataclasses.asdict(instance)


def with_changes(instance: Any, **changes: Any) -> Any:
    # replace() calls __init__ again, so __post_init__ validation reruns.
    return dataclasses.replace(instance, **changes)
