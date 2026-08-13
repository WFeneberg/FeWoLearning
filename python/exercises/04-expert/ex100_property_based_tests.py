"""Exercise 100 — a tiny property-based testing engine (expert).

Goal:   Build the three pieces a property-based testing library (Hypothesis, or
        .NET's FsCheck) is made of, in miniature: generators that produce random
        values of a given shape, a runner that tries a property against many of
        them, and shrinking — once a value falsifies the property, searching for a
        smaller value that still does, so the reported failure is the simplest one
        found rather than whatever random junk happened to trigger it.
Drills: generators as plain functions of a `random.Random` (so a run is
        reproducible from a seed), a shrink step that proposes simpler candidates
        and only keeps one that *still* falsifies the property, and iterating that
        shrink step to a fixed point rather than doing it once.
Passes: when `pytest exercises/04-expert/test_ex100_property_based_tests.py` is green.

Note:   shrinking here is greedy and local, not exhaustive — each round tries the
        current failing value's candidates in order and jumps to the first one
        that still fails, then repeats from there. That is enough to walk
        ``37 -> 18 -> 9 -> 8 -> 7 -> 6 -> 5`` down to the exact boundary of
        ``x < 5`` without ever trying every integer in between.
"""

from typing import Any, Callable, Iterator, TypeVar
import random

T = TypeVar("T")

Generator = Callable[[random.Random], T]


class FalsifiedError(Exception):
    """Raised by `for_all` once it has found and shrunk a counterexample."""

    def __init__(self, original: Any, shrunk: Any) -> None:
        self.original = original
        self.shrunk = shrunk
        super().__init__(f"property falsified: shrunk example = {shrunk!r} (original: {original!r})")


def ints(min_value: int = -100, max_value: int = 100) -> Generator:
    """Return a generator function producing random ints in `[min_value, max_value]`."""
    raise NotImplementedError


def lists(element_gen: Generator, max_size: int = 10) -> Generator:
    """Return a generator function producing a list of 0 to `max_size` elements,
    each drawn from `element_gen`."""
    raise NotImplementedError


def shrink_int(value: int) -> Iterator[int]:
    """Yield candidate simpler ints, moving toward zero: 0 itself first (unless
    `value` already is 0, which yields nothing), then repeatedly halving `value`
    towards zero for as long as its magnitude stays above 1, then finally `value`
    nudged one step closer to zero (``value - 1`` if positive, ``value + 1`` if
    negative)."""
    raise NotImplementedError


def shrink_list(value: list[Any]) -> Iterator[list[Any]]:
    """Yield candidate simpler lists: the empty list first (unless `value` already
    is empty, which yields nothing), then its first half, then its second half,
    then every version of `value` with exactly one element removed."""
    raise NotImplementedError


def shrink(value: Any) -> Iterator[Any]:
    """Dispatch to `shrink_int` or `shrink_list` by `value`'s type. A type this
    does not know how to shrink yields nothing — it is already as simple as this
    module can make it."""
    raise NotImplementedError


def for_all(
    generator: Generator,
    property_fn: Callable[[Any], bool],
    *,
    examples: int = 100,
    seed: int = 0,
) -> None:
    """Check that `property_fn` holds for `examples` values from `generator`.

    Draw values from a `random.Random(seed)` — the same seed must always draw the
    same sequence, which is what makes a failure reproducible. On the first value
    where `property_fn` returns a falsy result, shrink it: repeatedly scan
    `shrink(current)` for the first candidate that *still* falsifies `property_fn`,
    replace `current` with it, and go again — until a full pass over
    `shrink(current)` finds nothing that still falsifies it. Then raise
    `FalsifiedError(original=<the first failing value>, shrunk=<the final one>)`.

    If every example satisfies `property_fn`, return `None`.
    """
    raise NotImplementedError
