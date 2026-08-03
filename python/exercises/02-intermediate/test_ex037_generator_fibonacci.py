import itertools
from typing import Iterator

import pytest

from ex037_generator_fibonacci import (
    count_from,
    fib_up_to,
    fibonacci,
    is_exhausted,
    running_max,
    take,
)


def test_fibonacci_first_ten() -> None:
    assert list(itertools.islice(fibonacci(), 10)) == [0, 1, 1, 2, 3, 5, 8, 13, 21, 34]


def test_fibonacci_is_lazy_and_unbounded() -> None:
    # Reaching the 100th value at all proves nothing was precomputed into a list.
    value = next(itertools.islice(fibonacci(), 99, 100))
    assert value == 218922995834555169026


def test_fibonacci_instances_are_independent() -> None:
    a, b = fibonacci(), fibonacci()

    next(a)
    next(a)

    assert next(b) == 0


def test_fibonacci_returns_a_generator_not_a_list() -> None:
    result = fibonacci()
    assert iter(result) is result


@pytest.mark.parametrize(
    "source, count, expected",
    [
        ([1, 2, 3, 4], 2, [1, 2]),
        ([1, 2], 5, [1, 2]),
        ([1, 2], 0, []),
        ([1, 2], -1, []),
        ([], 3, []),
    ],
)
def test_take(source: list[int], count: int, expected: list[int]) -> None:
    assert take(iter(source), count) == expected


def test_take_consumes_only_what_it_returns() -> None:
    iterator = iter([1, 2, 3, 4])

    take(iterator, 2)

    assert list(iterator) == [3, 4]


def test_take_from_an_infinite_generator() -> None:
    assert take(fibonacci(), 5) == [0, 1, 1, 2, 3]


@pytest.mark.parametrize(
    "limit, expected",
    [
        (10, [0, 1, 1, 2, 3, 5, 8]),
        (0, [0]),
        (1, [0, 1, 1]),
        (-1, []),
    ],
)
def test_fib_up_to(limit: int, expected: list[int]) -> None:
    assert list(fib_up_to(limit)) == expected


def test_count_from_default_step() -> None:
    assert take(count_from(5), 4) == [5, 6, 7, 8]


def test_count_from_custom_step() -> None:
    assert take(count_from(0, 3), 4) == [0, 3, 6, 9]


def test_count_from_negative_step() -> None:
    assert take(count_from(10, -2), 3) == [10, 8, 6]


@pytest.mark.parametrize(
    "source, expected",
    [
        ([1, 3, 2], [1, 3, 3]),
        ([5, 4, 3], [5, 5, 5]),
        ([1], [1]),
        ([], []),
        ([-5, -1], [-5, -1]),
    ],
)
def test_running_max(source: list[int], expected: list[int]) -> None:
    assert list(running_max(iter(source))) == expected


def test_running_max_is_lazy() -> None:
    result = running_max(fibonacci())
    assert take(result, 5) == [0, 1, 1, 2, 3]


def test_is_exhausted_on_an_empty_iterator() -> None:
    assert is_exhausted(iter([])) is True


def test_is_exhausted_on_a_non_empty_iterator() -> None:
    assert is_exhausted(iter([1])) is False


def test_is_exhausted_consumes_one_item() -> None:
    iterator = iter([1, 2, 3])

    assert is_exhausted(iterator) is False
    # The first value is gone — that is the documented cost of asking.
    assert list(iterator) == [2, 3]


def test_generators_are_single_use() -> None:
    generator: Iterator[int] = fib_up_to(5)

    first = list(generator)
    second = list(generator)

    assert first == [0, 1, 1, 2, 3, 5]
    assert second == []
