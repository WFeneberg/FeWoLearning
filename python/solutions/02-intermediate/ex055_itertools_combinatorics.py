"""Exercise 055 — Combinatorics with itertools (reference solution)."""

import itertools
import math
from collections import Counter
from typing import Any, Iterable


def all_pairs(a: Iterable[Any], b: Iterable[Any]) -> list[tuple[Any, Any]]:
    # product's leftmost argument is the outer loop, so it varies slowest.
    return list(itertools.product(a, b))


def repeat_product(values: Iterable[Any], times: int) -> list[tuple[Any, ...]]:
    if times < 0:
        raise ValueError("repeat_product() times must not be negative")
    # repeat=0 yields exactly one empty tuple: the single way to choose nothing.
    return list(itertools.product(values, repeat=times))


def orderings(values: Iterable[Any]) -> list[tuple[Any, ...]]:
    return list(itertools.permutations(values))


def orderings_of_length(values: Iterable[Any], length: int) -> list[tuple[Any, ...]]:
    # permutations returns nothing when length exceeds the population, rather than
    # raising.
    return list(itertools.permutations(values, length))


def choose(values: Iterable[Any], count: int) -> list[tuple[Any, ...]]:
    # combinations keeps input order and never emits two orderings of one selection.
    return list(itertools.combinations(values, count))


def choose_with_repeats(values: Iterable[Any], count: int) -> list[tuple[Any, ...]]:
    return list(itertools.combinations_with_replacement(values, count))


def count_choices(total: int, count: int) -> int:
    if total < 0 or count < 0:
        raise ValueError("count_choices() arguments must not be negative")
    # math.comb is closed-form; enumerating combinations would be exponential.
    return math.comb(total, count)


def dice_sums(dice: int, sides: int = 6) -> dict[int, int]:
    if dice < 1:
        raise ValueError("dice_sums() dice must be at least 1")
    if sides < 1:
        raise ValueError("dice_sums() sides must be at least 1")
    faces = range(1, sides + 1)
    return dict(Counter(sum(roll) for roll in itertools.product(faces, repeat=dice)))
