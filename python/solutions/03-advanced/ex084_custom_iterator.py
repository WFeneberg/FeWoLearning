"""Exercise 084 — custom iterators (reference solution)."""

from typing import Any, Iterator, Sequence


class CountUpTo:
    def __init__(self, start: int, end: int) -> None:
        self._current = start
        self._end = end

    def __iter__(self) -> "CountUpTo":
        return self

    def __next__(self) -> int:
        if self._current > self._end:
            raise StopIteration
        value = self._current
        self._current += 1
        return value


class Batched:
    def __init__(self, data: Sequence[Any], size: int) -> None:
        if size <= 0:
            raise ValueError(f"size must be positive, got {size}")
        self._data = data
        self._size = size

    def __iter__(self) -> Iterator[list[Any]]:
        return _BatchIterator(self._data, self._size)


class _BatchIterator:
    def __init__(self, data: Sequence[Any], size: int) -> None:
        self._data = data
        self._size = size
        self._index = 0

    def __iter__(self) -> "_BatchIterator":
        return self

    def __next__(self) -> list[Any]:
        if self._index >= len(self._data):
            raise StopIteration
        chunk = list(self._data[self._index : self._index + self._size])
        self._index += self._size
        return chunk
