"""Exercise 010 — Slicing (reference solution)."""


def first_n(values: list[int], n: int) -> list[int]:
    # max(n, 0) keeps a negative n from being read as "all but the last |n|".
    return values[: max(n, 0)]


def last_n(values: list[int], n: int) -> list[int]:
    # values[-0:] is values[0:], i.e. everything — so 0 needs its own answer.
    if n <= 0:
        return []
    return values[-n:]


def middle(values: list[int]) -> list[int]:
    return values[1:-1]


def every_other(values: list[int]) -> list[int]:
    return values[::2]


def reversed_copy(values: list[int]) -> list[int]:
    return values[::-1]


def shallow_copy(values: list[int]) -> list[int]:
    return values[:]


def replace_slice(values: list[int], start: int, stop: int, replacement: list[int]) -> list[int]:
    # Slice assignment resizes the list, so the replacement need not match the
    # length of the slice it replaces.
    values[start:stop] = replacement
    return values


def chunk(values: list[int], size: int) -> list[list[int]]:
    if size <= 0:
        raise ValueError("chunk() size must be positive")
    return [values[i : i + size] for i in range(0, len(values), size)]
