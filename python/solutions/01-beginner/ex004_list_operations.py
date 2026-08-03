"""Exercise 004 — List operations (reference solution)."""

import bisect


def append_all(target: list[int], values: list[int]) -> list[int]:
    # extend mutates in place; target + values would build a new list instead.
    target.extend(values)
    return target


def insert_sorted(values: list[int], value: int) -> list[int]:
    result = values.copy()
    # bisect_right puts equal values after the existing ones.
    bisect.insort_right(result, value)
    return result


def pop_at(values: list[int], index: int) -> tuple[int, list[int]]:
    # list.pop already raises IndexError for an out-of-range index.
    return values.pop(index), values


def remove_first(values: list[int], value: int) -> bool:
    try:
        values.remove(value)
    except ValueError:
        return False
    return True


def sort_in_place_desc(values: list[int]) -> None:
    values.sort(reverse=True)


def flatten_once(nested: list[list[int]]) -> list[int]:
    return [item for inner in nested for item in inner]
