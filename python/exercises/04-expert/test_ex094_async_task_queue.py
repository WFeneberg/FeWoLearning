import asyncio

import pytest

from ex094_async_task_queue import run_task_queue, run


def make_task(value, events=None, delay=0):
    async def _task():
        if events is not None:
            events.append(f"start:{value}")
        await asyncio.sleep(delay)
        if events is not None:
            events.append(f"end:{value}")
        return value

    return _task


def test_results_come_back_in_the_original_task_order():
    tasks = [make_task(0), make_task(1), make_task(2)]

    results = run(run_task_queue(tasks, workers=3))

    assert [r.value for r in results] == [0, 1, 2]
    assert [r.task_id for r in results] == [0, 1, 2]


def test_empty_task_list_returns_empty():
    assert run(run_task_queue([], workers=2)) == []


def test_workers_must_be_positive():
    with pytest.raises(ValueError):
        run(run_task_queue([make_task(0)], workers=0))


def test_concurrency_never_exceeds_the_worker_count():
    running = 0
    peak = 0

    def make_tracked(value):
        async def _task():
            nonlocal running, peak
            running += 1
            peak = max(peak, running)
            await asyncio.sleep(0)
            running -= 1
            return value

        return _task

    tasks = [make_tracked(n) for n in range(6)]

    run(run_task_queue(tasks, workers=2))

    assert peak == 2


def test_a_failing_task_retries_then_succeeds():
    calls = []

    async def _flaky():
        calls.append(1)
        if len(calls) < 3:
            raise ValueError("not yet")
        return "ok"

    results = run(run_task_queue([_flaky], workers=1, max_retries=2))

    assert results[0].value == "ok"
    assert results[0].error is None
    assert results[0].attempts == 3


def test_a_task_that_always_fails_gives_up_after_max_retries():
    calls = []

    async def _always_fails():
        calls.append(1)
        raise ValueError("boom")

    results = run(run_task_queue([_always_fails], workers=1, max_retries=1))

    assert results[0].value is None
    assert isinstance(results[0].error, ValueError)
    assert results[0].attempts == 2
    assert len(calls) == 2


def test_default_max_retries_means_a_single_attempt():
    calls = []

    async def _always_fails():
        calls.append(1)
        raise ValueError("boom")

    results = run(run_task_queue([_always_fails], workers=1))

    assert results[0].attempts == 1
    assert len(calls) == 1


def test_one_failing_task_does_not_stop_the_others():
    async def _fails():
        raise ValueError("boom")

    tasks = [make_task(0), _fails, make_task(2)]

    results = run(run_task_queue(tasks, workers=3))

    assert results[0].value == 0
    assert results[1].error is not None
    assert results[2].value == 2


def test_a_bounded_queue_still_processes_more_tasks_than_its_size():
    tasks = [make_task(n) for n in range(5)]

    results = run(run_task_queue(tasks, workers=2, queue_size=1))

    assert [r.value for r in results] == [0, 1, 2, 3, 4]
