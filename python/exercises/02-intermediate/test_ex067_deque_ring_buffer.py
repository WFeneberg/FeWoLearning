import itertools
from collections import deque
from typing import Any

import pytest

from ex067_deque_ring_buffer import (
    TaskQueue,
    drain,
    last_n,
    ring_buffer,
    rotated,
    sliding_windows,
)


def test_ring_buffer_keeps_only_the_last_items() -> None:
    buffer = ring_buffer([1, 2, 3, 4, 5], 3)

    assert list(buffer) == [3, 4, 5]
    assert buffer.maxlen == 3


def test_ring_buffer_evicts_on_append() -> None:
    buffer = ring_buffer([1, 2, 3], 3)

    buffer.append(4)

    assert list(buffer) == [2, 3, 4]


def test_ring_buffer_not_yet_full() -> None:
    assert list(ring_buffer([1], 3)) == [1]


def test_ring_buffer_is_a_deque() -> None:
    assert isinstance(ring_buffer([1], 2), deque)


@pytest.mark.parametrize("size", [0, -1])
def test_ring_buffer_rejects_a_bad_size(size: int) -> None:
    with pytest.raises(ValueError):
        ring_buffer([1], size)


@pytest.mark.parametrize(
    "values, n, expected",
    [
        ([1, 2, 3, 4], 2, [3, 4]),
        ([1, 2], 5, [1, 2]),
        ([1, 2], 0, []),
        ([1, 2], -1, []),
        ([], 3, []),
    ],
)
def test_last_n(values: list[int], n: int, expected: list[int]) -> None:
    assert last_n(values, n) == expected


def test_last_n_works_on_a_large_lazy_source() -> None:
    # 200_000 items, but only three are ever retained.
    assert last_n(itertools.islice(itertools.count(), 200_000), 3) == [199_997, 199_998, 199_999]


@pytest.mark.parametrize(
    "values, steps, expected",
    [
        ([1, 2, 3, 4], 1, [4, 1, 2, 3]),
        ([1, 2, 3, 4], -1, [2, 3, 4, 1]),
        ([1, 2, 3, 4], 0, [1, 2, 3, 4]),
        ([1, 2, 3, 4], 5, [4, 1, 2, 3]),
        ([], 2, []),
        ([1], 3, [1]),
    ],
)
def test_rotated(values: list[int], steps: int, expected: list[int]) -> None:
    assert rotated(values, steps) == expected


@pytest.mark.parametrize(
    "values, size, expected",
    [
        ([1, 2, 3, 4], 2, [(1, 2), (2, 3), (3, 4)]),
        ([1, 2, 3], 3, [(1, 2, 3)]),
        ([1, 2], 3, []),
        ([], 2, []),
        ([1, 2, 3], 1, [(1,), (2,), (3,)]),
    ],
)
def test_sliding_windows(values: list[int], size: int, expected: list[tuple[Any, ...]]) -> None:
    assert sliding_windows(values, size) == expected


@pytest.mark.parametrize("size", [0, -1])
def test_sliding_windows_rejects_a_bad_size(size: int) -> None:
    with pytest.raises(ValueError):
        sliding_windows([1, 2], size)


def test_task_queue_is_fifo() -> None:
    queue = TaskQueue()
    queue.add("a")
    queue.add("b")

    assert queue.next_task() == "a"
    assert queue.next_task() == "b"


def test_task_queue_urgent_jumps_the_line() -> None:
    queue = TaskQueue()
    queue.add("normal")
    queue.add_urgent("urgent")

    assert queue.next_task() == "urgent"
    assert queue.next_task() == "normal"


def test_task_queue_len() -> None:
    queue = TaskQueue()

    assert len(queue) == 0
    queue.add("a")
    assert len(queue) == 1
    queue.next_task()
    assert len(queue) == 0


def test_task_queue_pop_when_empty_raises() -> None:
    with pytest.raises(IndexError):
        TaskQueue().next_task()


def test_task_queue_instances_are_independent() -> None:
    a, b = TaskQueue(), TaskQueue()
    a.add("x")

    assert len(b) == 0


def test_drain() -> None:
    queue = TaskQueue()
    queue.add("a")
    queue.add("b")
    queue.add_urgent("first")

    assert drain(queue) == ["first", "a", "b"]
    assert len(queue) == 0


def test_drain_an_empty_queue() -> None:
    assert drain(TaskQueue()) == []
