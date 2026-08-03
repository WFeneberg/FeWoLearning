"""Exercise 039 — yield from (intermediate).

Goal:   Delegate to a sub-iterator instead of re-yielding item by item.
Drills: `yield from`, recursive generators, flattening arbitrary nesting,
        the difference between yielding an iterable and delegating to it.
Passes: when `pytest exercises/02-intermediate/test_ex039_yield_from_flatten.py` is green.
"""

from typing import Any, Iterable, Iterator


def concat(*iterables: Iterable[int]) -> Iterator[int]:
    """Yield every item of every iterable, in order, using `yield from`.

    ``yield iterable`` would emit the iterables themselves; delegation emits their
    contents.
    """
    raise NotImplementedError


def flatten(nested: Iterable[Any]) -> Iterator[Any]:
    """Flatten arbitrarily deep nesting of lists and tuples.

    ``flatten([1, [2, [3, (4,)]]])`` yields 1, 2, 3, 4. Strings count as leaves — a
    str is iterable, so descending into it would recurse down to single characters
    and never stop making progress.
    """
    raise NotImplementedError


def flatten_depth(nested: Iterable[Any], depth: int = 1) -> Iterator[Any]:
    """Flatten only `depth` levels.

    ``flatten_depth([1, [2, [3]]], 1)`` yields 1, 2, [3]. A depth of 0 yields the
    input items unchanged. A negative depth raises ValueError.
    """
    raise NotImplementedError


def walk_tree(node: dict[str, Any]) -> Iterator[str]:
    """Yield the "name" of `node`, then of every descendant, depth-first.

    A node is ``{"name": str, "children": list[node]}``; "children" may be absent.
    """
    raise NotImplementedError


def interleave(a: Iterable[int], b: Iterable[int]) -> Iterator[int]:
    """Yield items alternately from `a` and `b` until both run out.

    ``interleave([1, 3], [2, 4, 6])`` yields 1, 2, 3, 4, 6 — the longer input's tail
    follows once the shorter one is exhausted.
    """
    raise NotImplementedError


def repeat_each(values: Iterable[int], times: int) -> Iterator[int]:
    """Yield each value `times` times in a row.

    A `times` of 0 or less yields nothing.
    """
    raise NotImplementedError
