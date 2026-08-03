"""Exercise 026 — Sorting by several keys (reference solution)."""

from collections import Counter
from operator import itemgetter

Record = tuple[str, int, str]


def by_city_then_name(records: list[Record]) -> list[Record]:
    # A tuple key compares element by element: city first, name only on a tie.
    return sorted(records, key=lambda r: (r[2], r[0]))


def by_age_desc_then_name(records: list[Record]) -> list[Record]:
    # Negating the age reverses just that field. reverse=True would also flip the
    # names, which is not what "age descending, then name ascending" means.
    return sorted(records, key=lambda r: (-r[1], r[0]))


def by_index(rows: list[tuple[int, str]], index: int) -> list[tuple[int, str]]:
    # itemgetter raises IndexError on its own for an out-of-range index.
    return sorted(rows, key=itemgetter(index))


def by_field_names(records: list[dict[str, object]], fields: list[str]) -> list[dict[str, object]]:
    if not fields:
        return list(records)
    # itemgetter with several names returns a tuple, which is exactly the key.
    return sorted(records, key=itemgetter(*fields))


def chained_sort(records: list[Record]) -> list[Record]:
    # Least significant key first. The second sort only reorders across cities, and
    # stability preserves the age order inside each city.
    by_age = sorted(records, key=itemgetter(1), reverse=True)
    return sorted(by_age, key=itemgetter(2))


def group_sizes(records: list[Record]) -> list[tuple[str, int]]:
    counts = Counter(city for _, _, city in records)
    return sorted(counts.items(), key=lambda item: (-item[1], item[0]))
