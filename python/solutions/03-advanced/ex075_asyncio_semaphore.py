"""Exercise 075 — asyncio.Semaphore (reference solution)."""

import asyncio
from typing import Any, Awaitable, Callable


async def run_limited(tasks: list[Callable[[], Awaitable[Any]]], limit: int) -> list[Any]:
    if limit < 1:
        raise ValueError("run_limited() limit must be at least 1")

    semaphore = asyncio.Semaphore(limit)

    async def guarded(task: Callable[[], Awaitable[Any]]) -> Any:
        # `async with` acquires on entry and releases on exit, including on an
        # exception — a manual acquire/release pair would leak a permit on failure.
        async with semaphore:
            return await task()

    return list(await asyncio.gather(*(guarded(task) for task in tasks)))


async def measure_peak(task_count: int, limit: int) -> int:
    semaphore = asyncio.Semaphore(limit)
    running = 0
    peak = 0

    async def worker() -> None:
        nonlocal running, peak
        async with semaphore:
            running += 1
            peak = max(peak, running)
            # Yielding inside the guarded section is what lets siblings pile up to the
            # limit; without it each task would finish before the next started.
            await asyncio.sleep(0)
            running -= 1

    await asyncio.gather(*(worker() for _ in range(task_count)))
    return peak


async def over_release_is_caught() -> bool:
    semaphore = asyncio.BoundedSemaphore(1)
    async with semaphore:
        pass
    try:
        # A plain Semaphore would silently raise its permit count to 2 here, so the
        # "limit" would stop limiting. BoundedSemaphore refuses instead.
        semaphore.release()
    except ValueError:
        return True
    return False


async def lock_serialises(task_count: int) -> int:
    lock = asyncio.Lock()
    running = 0
    peak = 0

    async def worker() -> None:
        nonlocal running, peak
        async with lock:
            running += 1
            peak = max(peak, running)
            await asyncio.sleep(0)
            running -= 1

    await asyncio.gather(*(worker() for _ in range(task_count)))
    return peak


async def acquisition_order(task_count: int) -> list[int]:
    semaphore = asyncio.Semaphore(1)
    order: list[int] = []

    async def worker(index: int) -> None:
        async with semaphore:
            order.append(index)
            await asyncio.sleep(0)

    # Creating the tasks in index order queues the waiters in that order, and asyncio
    # serves them first-come-first-served.
    tasks = [asyncio.create_task(worker(index)) for index in range(task_count)]
    await asyncio.gather(*tasks)
    return order


async def limit_does_not_limit_scheduling(task_count: int, limit: int) -> tuple[int, int]:
    semaphore = asyncio.Semaphore(limit)
    started = 0
    running = 0
    peak = 0

    async def worker() -> None:
        nonlocal started, running, peak
        # Counted before acquiring: every task really does start.
        started += 1
        async with semaphore:
            running += 1
            peak = max(peak, running)
            await asyncio.sleep(0)
            running -= 1

    await asyncio.gather(*(worker() for _ in range(task_count)))
    return started, peak


def run(coro: Awaitable[Any]) -> Any:
    return asyncio.run(coro)  # type: ignore[arg-type]
