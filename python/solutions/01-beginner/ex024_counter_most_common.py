"""Exercise 024 — collections.Counter (reference solution)."""

from collections import Counter


def char_counts(text: str) -> dict[str, int]:
    return dict(Counter(text))


def top_n(values: list[str], n: int) -> list[tuple[str, int]]:
    if n <= 0:
        # most_common(0) returns [] but most_common(-1) returns everything, so a
        # non-positive n needs its own answer.
        return []
    return Counter(values).most_common(n)


def most_common_value(values: list[str]) -> str | None:
    common = Counter(values).most_common(1)
    return common[0][0] if common else None


def duplicates(values: list[str]) -> list[str]:
    return sorted(value for value, count in Counter(values).items() if count > 1)


def merge_counts(a: dict[str, int], b: dict[str, int]) -> dict[str, int]:
    return dict(Counter(a) + Counter(b))


def difference(a: dict[str, int], b: dict[str, int]) -> dict[str, int]:
    # Counter's `-` drops anything at or below zero; .subtract() would keep it.
    return dict(Counter(a) - Counter(b))


def expand(counts: dict[str, int]) -> list[str]:
    # elements() repeats each key by its count and skips non-positive ones.
    return list(Counter(counts).elements())
