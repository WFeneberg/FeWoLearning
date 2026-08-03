"""Exercise 034 — List rotation (reference solution)."""


def rotate_left(values: list[int], amount: int) -> list[int]:
    if not values:
        # Guard the modulo: % 0 would raise.
        return []
    # Python's % is non-negative for a positive modulus, so a negative amount
    # becomes the equivalent right rotation for free.
    offset = amount % len(values)
    return values[offset:] + values[:offset]


def rotate_right(values: list[int], amount: int) -> list[int]:
    return rotate_left(values, -amount)


def rotate_in_place(values: list[int], amount: int) -> None:
    # Slice assignment writes through to the caller's list; `values = ...` would
    # only rebind the local name.
    values[:] = rotate_left(values, amount)


def swap(values: list[int], i: int, j: int) -> list[int]:
    # Both sides are evaluated before either is assigned, so no temporary needed.
    values[i], values[j] = values[j], values[i]
    return values


def move_to_front(values: list[int], index: int) -> list[int]:
    result = values.copy()
    # pop() validates the index (including negatives) and raises IndexError itself.
    result.insert(0, result.pop(index))
    return result


def chunk_rotate(values: list[int], size: int) -> list[int]:
    if size <= 0:
        raise ValueError("chunk_rotate() size must be positive")
    result: list[int] = []
    for start in range(0, len(values), size):
        chunk = values[start : start + size]
        result.extend(rotate_left(chunk, 1))
    return result
