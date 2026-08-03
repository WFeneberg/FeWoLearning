import asyncio

import pytest

from ex074_asyncio_queue_workers import (
    collect_ordered,
    drain_with_sentinel,
    process_all,
    queue_is_bounded,
    run,
    worker_error_stops_nothing,
)


async def double(n: int) -> int:
    await asyncio.sleep(0)
    return n * 2


def test_run_executes_a_coroutine() -> None:
    async def answer() -> int:
        return 1

    assert run(answer()) == 1


def test_process_all_handles_every_item() -> None:
    results = run(process_all([1, 2, 3, 4], 2, double))

    assert sorted(results) == [2, 4, 6, 8]


def test_process_all_with_one_worker() -> None:
    assert sorted(run(process_all([1, 2], 1, double))) == [2, 4]


def test_process_all_with_more_workers_than_items() -> None:
    assert sorted(run(process_all([1], 5, double))) == [2]


def test_process_all_empty() -> None:
    assert run(process_all([], 2, double)) == []


def test_process_all_terminates_rather_than_hanging_on_idle_workers() -> None:
    # Workers block on an empty queue; finishing at all proves they were shut down.
    assert len(run(process_all(list(range(20)), 4, double))) == 20


@pytest.mark.parametrize("worker_count", [0, -1])
def test_process_all_rejects_a_bad_worker_count(worker_count: int) -> None:
    with pytest.raises(ValueError):
        run(process_all([1], worker_count, double))


def test_collect_ordered_preserves_input_order() -> None:
    async def slow_for_small(n: int) -> int:
        # Smaller numbers finish last, so completion order differs from input order.
        await asyncio.sleep((10 - n) / 1000)
        return n * 10

    assert run(collect_ordered([1, 2, 3], 3, slow_for_small)) == [10, 20, 30]


def test_collect_ordered_simple() -> None:
    assert run(collect_ordered([1, 2, 3, 4], 2, double)) == [2, 4, 6, 8]


def test_collect_ordered_empty() -> None:
    assert run(collect_ordered([], 2, double)) == []


def test_queue_is_bounded() -> None:
    refused, fitted = run(queue_is_bounded(3))

    assert refused is True
    assert fitted == 3


def test_queue_is_bounded_size_one() -> None:
    assert run(queue_is_bounded(1)) == (True, 1)


def test_drain_with_sentinel_processes_everything() -> None:
    assert sorted(run(drain_with_sentinel([1, 2, 3], 2))) == [1, 2, 3]


def test_drain_with_sentinel_with_one_worker() -> None:
    assert sorted(run(drain_with_sentinel([5], 1))) == [5]


def test_drain_with_sentinel_empty() -> None:
    assert run(drain_with_sentinel([], 2)) == []


def test_worker_error_stops_nothing() -> None:
    successes, errors = run(worker_error_stops_nothing([1, 2, 3, 4]))

    assert sorted(successes) == [2, 4]
    assert errors == 2


def test_worker_error_stops_nothing_all_even() -> None:
    successes, errors = run(worker_error_stops_nothing([2, 4]))

    assert sorted(successes) == [2, 4]
    assert errors == 0


def test_worker_error_stops_nothing_all_odd() -> None:
    successes, errors = run(worker_error_stops_nothing([1, 3, 5]))

    assert successes == []
    assert errors == 3


def test_worker_error_does_not_hang_the_join() -> None:
    # Missing task_done() in the error path would leave join() waiting forever.
    assert run(worker_error_stops_nothing([1, 1, 1, 1, 1])) == ([], 5)
