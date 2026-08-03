import asyncio
from typing import Any, Awaitable, Callable

import pytest

from ex075_asyncio_semaphore import (
    acquisition_order,
    limit_does_not_limit_scheduling,
    lock_serialises,
    measure_peak,
    over_release_is_caught,
    run,
    run_limited,
)


def make_task(value: int) -> Callable[[], Awaitable[int]]:
    async def task() -> int:
        await asyncio.sleep(0)
        return value * 10

    return task


def test_run_executes_a_coroutine() -> None:
    async def answer() -> int:
        return 3

    assert run(answer()) == 3


def test_run_limited_results_in_input_order() -> None:
    tasks = [make_task(n) for n in range(5)]

    assert run(run_limited(tasks, 2)) == [0, 10, 20, 30, 40]


def test_run_limited_with_a_limit_above_the_count() -> None:
    assert run(run_limited([make_task(1), make_task(2)], 10)) == [10, 20]


def test_run_limited_empty() -> None:
    assert run(run_limited([], 2)) == []


@pytest.mark.parametrize("limit", [0, -1])
def test_run_limited_rejects_a_bad_limit(limit: int) -> None:
    with pytest.raises(ValueError):
        run(run_limited([make_task(1)], limit))


@pytest.mark.parametrize("task_count, limit, expected", [(10, 3, 3), (10, 1, 1), (5, 5, 5)])
def test_measure_peak_equals_the_limit(task_count: int, limit: int, expected: int) -> None:
    assert run(measure_peak(task_count, limit)) == expected


def test_measure_peak_cannot_exceed_the_task_count() -> None:
    assert run(measure_peak(2, 10)) == 2


def test_over_release_is_caught() -> None:
    assert run(over_release_is_caught()) is True


@pytest.mark.parametrize("task_count", [1, 5, 20])
def test_lock_serialises_to_one(task_count: int) -> None:
    assert run(lock_serialises(task_count)) == 1


def test_acquisition_order_is_first_come_first_served() -> None:
    assert run(acquisition_order(5)) == [0, 1, 2, 3, 4]


def test_acquisition_order_single() -> None:
    assert run(acquisition_order(1)) == [0]


def test_limit_does_not_limit_scheduling() -> None:
    started, peak = run(limit_does_not_limit_scheduling(8, 3))

    # Everything is scheduled; only the guarded section is capped.
    assert started == 8
    assert peak == 3


def test_limit_does_not_limit_scheduling_with_a_wide_limit() -> None:
    started, peak = run(limit_does_not_limit_scheduling(4, 10))

    assert started == 4
    assert peak == 4
