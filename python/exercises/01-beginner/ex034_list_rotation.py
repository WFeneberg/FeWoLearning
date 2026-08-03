"""Exercise 034 — List rotation (beginner).

Goal:   Move items around inside a list, in place and as copies.
Drills: slice assignment, modulo to normalise an over-long rotation, negative
        amounts, deque.rotate as the ready-made answer, swapping via unpacking.
Passes: when `pytest exercises/01-beginner/test_ex034_list_rotation.py` is green.
"""


def rotate_left(values: list[int], amount: int) -> list[int]:
    """Return a **new** list rotated left by `amount`.

    ``rotate_left([1, 2, 3, 4], 1)`` -> ``[2, 3, 4, 1]``. An `amount` larger than the
    list wraps (use modulo), and a negative `amount` rotates right. An empty list
    yields an empty list rather than raising on the modulo.
    """
    raise NotImplementedError


def rotate_right(values: list[int], amount: int) -> list[int]:
    """Return a new list rotated right by `amount`. Same wrapping rules."""
    raise NotImplementedError


def rotate_in_place(values: list[int], amount: int) -> None:
    """Rotate left by `amount` **in place**, returning None.

    Assign to the full slice (``values[:] = ...``) rather than rebinding the name,
    which would leave the caller's list untouched.
    """
    raise NotImplementedError


def swap(values: list[int], i: int, j: int) -> list[int]:
    """Swap two positions in place and return the list.

    Use tuple unpacking, not a temporary variable. An out-of-range index raises
    IndexError.
    """
    raise NotImplementedError


def move_to_front(values: list[int], index: int) -> list[int]:
    """Return a new list with the item at `index` moved to the front.

    An out-of-range index raises IndexError. Negative indices count from the end.
    """
    raise NotImplementedError


def chunk_rotate(values: list[int], size: int) -> list[int]:
    """Rotate each consecutive chunk of `size` items left by one, as a new list.

    ``chunk_rotate([1, 2, 3, 4, 5], 2)`` -> ``[2, 1, 4, 3, 5]``. A final short chunk
    is rotated within itself. A `size` of 0 or less raises ValueError.
    """
    raise NotImplementedError
