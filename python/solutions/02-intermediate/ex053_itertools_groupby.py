"""Exercise 053 — itertools.groupby (reference solution)."""

import itertools
from typing import Any, Callable, Iterable


def runs(values: Iterable[Any]) -> list[tuple[Any, int]]:
    # No sort: consecutive runs are exactly what is wanted here.
    return [(value, sum(1 for _ in group)) for value, group in itertools.groupby(values)]


def group_sorted(values: Iterable[Any], key: Callable[[Any], Any]) -> dict[Any, list[Any]]:
    # groupby only ever sees adjacent items, so equal keys have to be brought
    # together first. Skipping this sort is the classic groupby bug.
    ordered = sorted(values, key=key)
    return {k: list(group) for k, group in itertools.groupby(ordered, key=key)}


def group_lengths(words: Iterable[str]) -> dict[int, list[str]]:
    # A plain dict walk keeps each bucket in the input's own order, which sorting
    # for groupby would destroy.
    groups: dict[int, list[str]] = {}
    for word in words:
        groups.setdefault(len(word), []).append(word)
    return groups


def compress(text: str) -> str:
    return "".join(f"{value}{count}" for value, count in runs(text))


def longest_run(values: Iterable[Any]) -> tuple[Any, int] | None:
    # max() keeps the first item on a tie, which is the documented behaviour.
    return max(runs(values), key=lambda pair: pair[1], default=None)


def first_of_each_run(values: Iterable[Any]) -> list[Any]:
    return [value for value, _group in itertools.groupby(values)]


def count_groups(values: Iterable[Any], key: Callable[[Any], Any]) -> int:
    # Counting the keys only: advancing to the next group invalidates the previous
    # group's iterator, so nothing here may touch the contents.
    return sum(1 for _key, _group in itertools.groupby(values, key=key))
