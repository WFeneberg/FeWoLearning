"""Exercise 065 — defaultdict (reference solution)."""

from collections import defaultdict
from typing import Any, Callable, Iterable


def group_by(items: Iterable[Any], key: Callable[[Any], Any]) -> dict[Any, list[Any]]:
    groups: defaultdict[Any, list[Any]] = defaultdict(list)
    for item in items:
        groups[key(item)].append(item)
    # dict(...) so the caller gets normal KeyError behaviour back.
    return dict(groups)


def count_by(items: Iterable[Any], key: Callable[[Any], Any]) -> dict[Any, int]:
    counts: defaultdict[Any, int] = defaultdict(int)
    for item in items:
        counts[key(item)] += 1
    return dict(counts)


def collect_unique(pairs: Iterable[tuple[Any, Any]]) -> dict[Any, set[Any]]:
    collected: defaultdict[Any, set[Any]] = defaultdict(set)
    for key, value in pairs:
        collected[key].add(value)
    return dict(collected)


def nested_counts(triples: Iterable[tuple[str, str, int]]) -> dict[str, dict[str, int]]:
    # The factory has to *return* a defaultdict, so it must be a callable that builds
    # one — `defaultdict(defaultdict(int))` would pass an instance, not a factory.
    totals: defaultdict[str, defaultdict[str, int]] = defaultdict(lambda: defaultdict(int))
    for outer, inner, amount in triples:
        totals[outer][inner] += amount
    return {outer: dict(inner) for outer, inner in totals.items()}


def invert_multi(mapping: dict[str, list[str]]) -> dict[str, list[str]]:
    inverted: defaultdict[str, list[str]] = defaultdict(list)
    for key, values in mapping.items():
        for value in values:
            inverted[value].append(key)
    return dict(inverted)


def reads_create_keys() -> tuple[int, list[str]]:
    demo: defaultdict[str, list[str]] = defaultdict(list)
    # A bare read inserts the key: __missing__ calls the factory and stores the
    # result. This is the trap when a defaultdict is used as a lookup table.
    demo["absent"]
    return len(demo), sorted(demo)


def safe_lookup(store: dict[str, list[str]], key: str) -> list[str]:
    # .get never inserts, which is why it is right for a read-only lookup.
    return store.get(key, [])
