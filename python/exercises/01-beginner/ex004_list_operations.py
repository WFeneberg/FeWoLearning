"""Exercise 004 — List operations (beginner).

Goal:   Mutate and query lists with the built-in list methods.
Drills: append/extend/insert/pop/remove, in-place sorting, copies vs aliases.
Passes: when `pytest exercises/01-beginner/test_ex004_list_operations.py` is green.
"""


def append_all(target: list[int], values: list[int]) -> list[int]:
    """Append every value to `target` **in place** and return the same list object.

    The returned object must be `target` itself, not a copy.
    """
    raise NotImplementedError


def insert_sorted(values: list[int], value: int) -> list[int]:
    """Return a **new** list with `value` inserted so an already-sorted input stays
    sorted. `values` must not be modified.

    ``insert_sorted([1, 3, 5], 4)`` -> ``[1, 3, 4, 5]``. Equal values go after the
    existing ones.
    """
    raise NotImplementedError


def pop_at(values: list[int], index: int) -> tuple[int, list[int]]:
    """Remove the item at `index` in place and return ``(removed, values)``.

    An out-of-range index must raise IndexError.
    """
    raise NotImplementedError


def remove_first(values: list[int], value: int) -> bool:
    """Remove the first occurrence of `value` in place.

    Return True when something was removed, False when the value was absent —
    never raise.
    """
    raise NotImplementedError


def sort_in_place_desc(values: list[int]) -> None:
    """Sort `values` in place, largest first. Returns None, like list.sort does."""
    raise NotImplementedError


def flatten_once(nested: list[list[int]]) -> list[int]:
    """Concatenate one level of nesting into a new flat list.

    ``flatten_once([[1, 2], [], [3]])`` -> ``[1, 2, 3]``.
    """
    raise NotImplementedError
