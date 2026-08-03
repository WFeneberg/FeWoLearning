"""Exercise 056 — functools.reduce (reference solution)."""

import functools
import itertools
import operator
from typing import Any, Callable, Iterable


def product(values: Iterable[int]) -> int:
    # The initial value 1 is what makes the empty input work: without it reduce
    # raises TypeError on an empty sequence.
    return functools.reduce(operator.mul, values, 1)


def total(values: Iterable[int], start: int = 0) -> int:
    return functools.reduce(operator.add, values, start)


def longest(words: Iterable[str]) -> str:
    # `>` rather than `>=` keeps the first of equal-length words.
    return functools.reduce(lambda best, word: word if len(word) > len(best) else best, words, "")


def merge_dicts(mappings: Iterable[dict[str, int]]) -> dict[str, int]:
    # {**acc, **m} builds a new dict each step, so no input is touched.
    return functools.reduce(lambda acc, m: {**acc, **m}, mappings, {})


def compose(*funcs: Callable[[Any], Any]) -> Callable[[Any], Any]:
    def composed(value: Any) -> Any:
        # Folding the *value* through the functions gives left-to-right order.
        return functools.reduce(lambda acc, func: func(acc), funcs, value)

    return composed


def flatten(nested: Iterable[list[Any]]) -> list[Any]:
    # O(n²): each step allocates a new list. Kept deliberately to show the fold;
    # itertools.chain.from_iterable is the right tool in real code.
    return functools.reduce(operator.add, nested, [])


def running(values: Iterable[int], func: Callable[[int, int], int]) -> list[int]:
    # accumulate keeps every intermediate; reduce would discard all but the last.
    return list(itertools.accumulate(values, func))


def fold_right(values: list[int], func: Callable[[int, int], int], initial: int) -> int:
    # reduce only folds left, so reverse the input and swap the arguments: the
    # accumulator has to arrive on the right-hand side.
    return functools.reduce(lambda acc, value: func(value, acc), reversed(values), initial)
