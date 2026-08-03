"""Exercise 069 — Unpacking operators (reference solution)."""

from typing import Any, Callable, Iterable


def call_with_list(func: Callable[..., Any], args: list[Any]) -> Any:
    return func(*args)


def call_with_dict(func: Callable[..., Any], kwargs: dict[str, Any]) -> Any:
    return func(**kwargs)


def concat_lists(*lists: list[Any]) -> list[Any]:
    # [*a, *b, …] accepts any iterable, where a + b would demand lists.
    return [item for sublist in lists for item in sublist]


def merge(*mappings: dict[str, Any]) -> dict[str, Any]:
    result: dict[str, Any] = {}
    for mapping in mappings:
        # A fresh dict each step, so no argument is touched.
        result = {**result, **mapping}
    return result


def merge_with_extra(base: dict[str, Any], **extra: Any) -> dict[str, Any]:
    # Later keys win, so the keywords override the base.
    return {**base, **extra}


def split_first_rest(values: Iterable[Any]) -> tuple[Any, list[Any]]:
    items = list(values)
    if not items:
        raise ValueError("split_first_rest() needs at least one value")
    first, *rest = items
    return first, rest


def split_ends(values: list[Any]) -> tuple[Any, list[Any], Any]:
    if len(values) < 2:
        # `first, *middle, last = …` needs two items to bind both ends.
        raise ValueError("split_ends() needs at least two values")
    first, *middle, last = values
    return first, middle, last


def to_set_literal(*iterables: Iterable[Any]) -> set[Any]:
    if not iterables:
        # There is no empty set literal — {} is a dict — so set() it is.
        return set()
    return {item for iterable in iterables for item in iterable}


def forward_all(func: Callable[..., Any], args: list[Any], kwargs: dict[str, Any]) -> Any:
    return func(*args, **kwargs)
