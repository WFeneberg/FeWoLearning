"""Exercise 055 — Combinatorics with itertools (intermediate).

Goal:   Enumerate combinations without writing nested loops.
Drills: product, permutations, combinations, combinations_with_replacement,
        the difference between order mattering and not, counting results,
        and that these are lazy so their sizes must be bounded deliberately.
Passes: when `pytest exercises/02-intermediate/test_ex055_itertools_combinatorics.py` is green.
"""

from typing import Any, Iterable


def all_pairs(a: Iterable[Any], b: Iterable[Any]) -> list[tuple[Any, Any]]:
    """Every combination of one item from each input, first varying slowest."""
    raise NotImplementedError


def repeat_product(values: Iterable[Any], times: int) -> list[tuple[Any, ...]]:
    """All `times`-length tuples drawn from `values`, with repetition and order.

    ``repeat_product("ab", 2)`` gives 4 results. A `times` of 0 gives one empty
    tuple — the single way to choose nothing. A negative `times` raises ValueError.
    """
    raise NotImplementedError


def orderings(values: Iterable[Any]) -> list[tuple[Any, ...]]:
    """Every full-length ordering (permutation) of `values`."""
    raise NotImplementedError


def orderings_of_length(values: Iterable[Any], length: int) -> list[tuple[Any, ...]]:
    """Every ordered selection of exactly `length` items, without repetition.

    A `length` above the input size yields [], not an error.
    """
    raise NotImplementedError


def choose(values: Iterable[Any], count: int) -> list[tuple[Any, ...]]:
    """Every unordered selection of `count` distinct items, in input order.

    ``choose("abc", 2)`` gives ``("a","b"), ("a","c"), ("b","c")`` — ``("b","a")`` is
    the same selection and does not appear.
    """
    raise NotImplementedError


def choose_with_repeats(values: Iterable[Any], count: int) -> list[tuple[Any, ...]]:
    """Every unordered selection of `count` items where repeats are allowed."""
    raise NotImplementedError


def count_choices(total: int, count: int) -> int:
    """Return the binomial coefficient "total choose count", without enumerating.

    Use ``math.comb``: enumerating would be exponential for no reason. A negative
    argument raises ValueError.
    """
    raise NotImplementedError


def dice_sums(dice: int, sides: int = 6) -> dict[int, int]:
    """Map each achievable total to how many rolls produce it.

    ``dice_sums(2)`` gives 11 entries from 2 to 12, with 7 the most frequent at 6.
    A `dice` or `sides` below 1 raises ValueError.
    """
    raise NotImplementedError
