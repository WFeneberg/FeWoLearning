"""Exercise 005 — Tuple unpacking (reference solution)."""


def swap(pair: tuple[int, int]) -> tuple[int, int]:
    first, second = pair
    return second, first


def head_tail(values: list[int]) -> tuple[int, list[int]]:
    if not values:
        raise ValueError("head_tail() needs at least one value")
    head, *tail = values
    return head, tail


def first_last(values: list[int]) -> tuple[int, int]:
    if not values:
        raise ValueError("first_last() needs at least one value")
    if len(values) == 1:
        # `first, *middle, last = [x]` would raise: the pattern needs two items.
        only = values[0]
        return only, only
    # The star-target absorbs everything between the ends, including nothing.
    first, *_middle, last = values
    return first, last


def min_max(values: list[int]) -> tuple[int, int]:
    if not values:
        raise ValueError("min_max() needs at least one value")
    return min(values), max(values)


def unpack_record(record: tuple[str, tuple[int, int]]) -> tuple[str, int, int]:
    name, (x, y) = record
    return name, x, y


def divmod_pairs(values: list[int], divisor: int) -> list[tuple[int, int]]:
    if divisor == 0:
        raise ZeroDivisionError("divmod_pairs() divisor must not be zero")
    return [divmod(value, divisor) for value in values]
