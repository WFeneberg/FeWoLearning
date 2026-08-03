"""Exercise 006 — Set operations (reference solution)."""


def dedupe_keep_order(values: list[str]) -> list[str]:
    seen: set[str] = set()
    result: list[str] = []
    for value in values:
        if value not in seen:
            seen.add(value)
            result.append(value)
    return result


def common_elements(a: list[int], b: list[int]) -> set[int]:
    return set(a) & set(b)


def only_in_first(a: list[int], b: list[int]) -> set[int]:
    return set(a) - set(b)


def symmetric_difference(a: list[int], b: list[int]) -> set[int]:
    return set(a) ^ set(b)


def is_subset(small: list[int], large: list[int]) -> bool:
    return set(small) <= set(large)


def has_duplicates(values: list[int]) -> bool:
    # A set collapses repeats, so a shorter set means something occurred twice.
    return len(set(values)) != len(values)


def group_key(tags: list[str]) -> frozenset[str]:
    # frozenset is hashable (unlike set), so it can be a dict key, and it ignores
    # both order and repeats.
    return frozenset(tags)
