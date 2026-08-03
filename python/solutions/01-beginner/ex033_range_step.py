"""Exercise 033 — range and reversed (reference solution)."""


def evens_up_to(limit: int) -> list[int]:
    if limit < 0:
        return []
    # stop is exclusive, so +1 makes an even `limit` itself appear.
    return list(range(0, limit + 1, 2))


def countdown(start: int) -> list[int]:
    # Stopping at 0 (exclusive) with step -1 ends at 1.
    return list(range(start, 0, -1))


def every_nth(values: list[str], n: int) -> list[str]:
    if n <= 0:
        raise ValueError("every_nth() n must be positive")
    return [values[i] for i in range(0, len(values), n)]


def indices_reversed(values: list[str]) -> list[int]:
    # reversed() over a range stays lazy — no intermediate list is built.
    return list(reversed(range(len(values))))


def arithmetic_series(start: int, step: int, count: int) -> list[int]:
    if count < 0:
        return []
    # range() cannot be used here: a step of 0 is illegal for it, but the spec asks
    # for `count` copies of `start` in that case.
    return [start + step * i for i in range(count)]


def is_in_range(value: int, start: int, stop: int, step: int) -> bool:
    if step == 0:
        raise ValueError("is_in_range() step must not be zero")
    # `in` on a range of ints is arithmetic, not a scan.
    return value in range(start, stop, step)


def sum_multiples(limit: int, divisor: int) -> int:
    if divisor <= 0:
        raise ValueError("sum_multiples() divisor must be positive")
    return sum(range(divisor, limit, divisor))
