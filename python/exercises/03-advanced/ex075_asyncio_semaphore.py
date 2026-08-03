"""Exercise 075 — asyncio.Semaphore (advanced).

Goal:   Cap how much runs at once without capping how much is scheduled.
Drills: Semaphore as an async context manager, observing the concurrency peak,
        BoundedSemaphore catching an over-release, Lock as a semaphore of one,
        and fairness (waiters are served in arrival order).
Passes: when `pytest exercises/03-advanced/test_ex075_asyncio_semaphore.py` is green.
"""

import asyncio
from typing import Any, Awaitable, Callable


async def run_limited(
    tasks: list[Callable[[], Awaitable[Any]]], limit: int
) -> list[Any]:
    """Run every task, at most `limit` concurrently, results in input order.

    A `limit` below 1 raises ValueError.
    """
    raise NotImplementedError


async def measure_peak(task_count: int, limit: int) -> int:
    """Return the highest number of tasks that were ever running at once.

    Must equal `limit` when ``task_count >= limit``. Each task increments a counter,
    yields to the loop, then decrements.
    """
    raise NotImplementedError


async def over_release_is_caught() -> bool:
    """Report whether releasing a BoundedSemaphore too often raises ValueError.

    A plain Semaphore silently grows its permit count when over-released, which turns a
    concurrency bug into a limit that quietly stops limiting. BoundedSemaphore refuses.
    """
    raise NotImplementedError


async def lock_serialises(task_count: int) -> int:
    """Run `task_count` tasks under one Lock and return the observed peak.

    A Lock is a semaphore of one, so the answer is always 1.
    """
    raise NotImplementedError


async def acquisition_order(task_count: int) -> list[int]:
    """Return the order in which waiters acquired a semaphore of one.

    Tasks are started in index order and all queue on the same semaphore, so they must
    be served first-come-first-served.
    """
    raise NotImplementedError


async def limit_does_not_limit_scheduling(task_count: int, limit: int) -> tuple[int, int]:
    """Return ``(tasks_started, peak_inside_the_limit)``.

    Every task starts — scheduling is unrestricted — but only `limit` are ever inside
    the guarded section. The distinction is the whole point of a semaphore.
    """
    raise NotImplementedError


def run(coro: Awaitable[Any]) -> Any:
    """Run a coroutine to completion."""
    raise NotImplementedError
