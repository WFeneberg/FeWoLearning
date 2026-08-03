"""Exercise 070 — Comparator-based sorting (intermediate).

Goal:   Sort by a rule that cannot be expressed as a key function.
Drills: functools.cmp_to_key, the comparator contract (negative/zero/positive),
        why key= is preferred when it is possible, and building a total order out of
        several criteria.
Passes: when `pytest exercises/02-intermediate/test_ex070_cmp_to_key_sorting.py` is green.
"""

from typing import Any, Callable

Record = dict[str, Any]


def compare_lengths(a: str, b: str) -> int:
    """A comparator: negative if `a` is shorter, positive if longer, 0 if equal.

    This is the contract every comparator must satisfy — the sign is what matters, not
    the magnitude.
    """
    raise NotImplementedError


def sort_by_comparator(values: list[str], comparator: Callable[[str, str], int]) -> list[str]:
    """Sort using a two-argument comparator, via ``functools.cmp_to_key``.

    Python 3 dropped ``sorted(cmp=…)``; cmp_to_key wraps a comparator into the key
    object the sort actually wants.
    """
    raise NotImplementedError


def sort_largest_concatenation(numbers: list[int]) -> str:
    """Arrange the numbers so their concatenation is the largest possible number.

    ``[3, 30, 34, 5, 9]`` -> ``"9534330"``. This is the classic case where no key
    function works: whether 3 belongs before 30 depends on comparing "330" with "303",
    which is a property of the *pair*, not of either value alone.
    """
    raise NotImplementedError


def sort_version_strings(versions: list[str]) -> list[str]:
    """Sort dotted version strings numerically, ascending.

    ``"1.10.0"`` must come after ``"1.9.0"``, which plain string ordering gets wrong.
    Segments counts may differ: ``"1.2"`` sorts before ``"1.2.1"``.
    """
    raise NotImplementedError


def sort_records(records: list[Record]) -> list[Record]:
    """Sort by ``priority`` descending, then ``name`` ascending, using a comparator.

    Expressible with a key too — the point here is writing the equivalent comparator
    and seeing how much more code it takes.
    """
    raise NotImplementedError


def is_valid_comparator(comparator: Callable[[Any, Any], int], samples: list[Any]) -> bool:
    """Check the comparator is antisymmetric over `samples`.

    For every pair, ``sign(cmp(a, b))`` must equal ``-sign(cmp(b, a))``. A comparator
    that fails this makes the sort order undefined rather than raising.
    """
    raise NotImplementedError
