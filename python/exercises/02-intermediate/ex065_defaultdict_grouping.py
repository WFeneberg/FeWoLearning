"""Exercise 065 — defaultdict (intermediate).

Goal:   Accumulate into a dict without checking whether each key exists yet.
Drills: defaultdict with list/int/set/dict factories, the surprise that reading a
        missing key *creates* it, converting back to a plain dict, and when
        setdefault is the better choice.
Passes: when `pytest exercises/02-intermediate/test_ex065_defaultdict_grouping.py` is green.
"""

from typing import Any, Callable, Iterable


def group_by(items: Iterable[Any], key: Callable[[Any], Any]) -> dict[Any, list[Any]]:
    """Group items by `key`, returning a **plain** dict.

    Returning the defaultdict itself would leak its surprising behaviour to the
    caller: reading an absent key would silently insert it.
    """
    raise NotImplementedError


def count_by(items: Iterable[Any], key: Callable[[Any], Any]) -> dict[Any, int]:
    """Count items per key, using an int factory. Returns a plain dict."""
    raise NotImplementedError


def collect_unique(pairs: Iterable[tuple[Any, Any]]) -> dict[Any, set[Any]]:
    """Collect the values per key into sets, dropping duplicates."""
    raise NotImplementedError


def nested_counts(triples: Iterable[tuple[str, str, int]]) -> dict[str, dict[str, int]]:
    """Sum the third element per ``(outer, inner)`` pair, two levels deep.

    A nested defaultdict needs a factory that itself returns a defaultdict — that is
    what a lambda or ``partial`` is for here. The result must be plain dicts all the
    way down.
    """
    raise NotImplementedError


def invert_multi(mapping: dict[str, list[str]]) -> dict[str, list[str]]:
    """Invert a one-to-many mapping.

    ``{"a": ["x", "y"], "b": ["x"]}`` -> ``{"x": ["a", "b"], "y": ["a"]}``. Keys appear
    in first-seen order.
    """
    raise NotImplementedError


def reads_create_keys() -> tuple[int, list[str]]:
    """Demonstrate the defaultdict surprise.

    Create a ``defaultdict(list)``, read one absent key **without** assigning, and
    return ``(len(d), sorted(d))`` afterwards. The answer is ``(1, ["absent"])``: a
    plain read inserted it, which is exactly the trap when using a defaultdict as a
    lookup table.
    """
    raise NotImplementedError


def safe_lookup(store: dict[str, list[str]], key: str) -> list[str]:
    """Return ``store[key]`` or an empty list, **without** inserting the key.

    ``.get(key, [])`` is right here; a defaultdict read would grow the dict.
    """
    raise NotImplementedError
