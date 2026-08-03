"""Exercise 026 — Sorting by several keys (beginner).

Goal:   Order records by more than one field, in either direction.
Drills: tuple keys, operator.itemgetter/attrgetter, mixing ascending and
        descending, and why chained sorts work because the sort is stable.
Passes: when `pytest exercises/01-beginner/test_ex026_sort_multiple_keys.py` is green.
"""

Record = tuple[str, int, str]
"""A record is ``(name, age, city)``."""


def by_city_then_name(records: list[Record]) -> list[Record]:
    """Sort by city, then by name, both ascending. Use a tuple key."""
    raise NotImplementedError


def by_age_desc_then_name(records: list[Record]) -> list[Record]:
    """Sort by age descending, then name ascending.

    `reverse=True` cannot do this: it would flip the names too. Negate the numeric
    field in the key instead.
    """
    raise NotImplementedError


def by_index(rows: list[tuple[int, str]], index: int) -> list[tuple[int, str]]:
    """Sort tuples by the element at `index`, using ``operator.itemgetter``.

    An index outside the tuples raises IndexError.
    """
    raise NotImplementedError


def by_field_names(records: list[dict[str, object]], fields: list[str]) -> list[dict[str, object]]:
    """Sort dicts by the given field names, in order, all ascending.

    ``itemgetter(*fields)`` builds the tuple key for you. No fields means the input
    order is kept.
    """
    raise NotImplementedError


def chained_sort(records: list[Record]) -> list[Record]:
    """Achieve "city ascending, age descending" with two successive sorts.

    Sort by the *least* significant key first and the most significant last: since
    the sort is stable, the earlier ordering survives inside equal groups.
    """
    raise NotImplementedError


def group_sizes(records: list[Record]) -> list[tuple[str, int]]:
    """Return ``(city, count)`` sorted by count descending, then city ascending."""
    raise NotImplementedError
