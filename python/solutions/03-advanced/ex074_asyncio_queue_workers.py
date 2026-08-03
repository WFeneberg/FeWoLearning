"""Exercise 074 — asyncio.Queue and worker tasks (reference solution)."""

import asyncio
import contextlib
from typing import Any, Awaitable, Callable


async def process_all(
    items: list[int], worker_count: int, handler: Callable[[int], Awaitable[Any]]
) -> list[Any]:
    if worker_count < 1:
        raise ValueError("process_all() worker_count must be at least 1")

    queue: asyncio.Queue[int] = asyncio.Queue()
    results: list[Any] = []

    async def worker() -> None:
        while True:
            item = await queue.get()
            try:
                results.append(await handler(item))
            finally:
                # In finally so a handler failure still balances the join() count.
                queue.task_done()

    workers = [asyncio.create_task(worker()) for _ in range(worker_count)]
    for item in items:
        queue.put_nowait(item)

    # join() waits for the *work*, not for the workers — which never finish on their
    # own, since they block on an empty queue.
    await queue.join()
    for task in workers:
        task.cancel()
    await asyncio.gather(*workers, return_exceptions=True)

    return results


async def collect_ordered(
    items: list[int], worker_count: int, handler: Callable[[int], Awaitable[Any]]
) -> list[Any]:
    if worker_count < 1:
        raise ValueError("collect_ordered() worker_count must be at least 1")

    queue: asyncio.Queue[tuple[int, int]] = asyncio.Queue()
    # Pre-sized, so each worker writes to its own slot and order survives
    # out-of-order completion.
    results: list[Any] = [None] * len(items)

    async def worker() -> None:
        while True:
            index, item = await queue.get()
            try:
                results[index] = await handler(item)
            finally:
                queue.task_done()

    workers = [asyncio.create_task(worker()) for _ in range(worker_count)]
    for pair in enumerate(items):
        queue.put_nowait(pair)

    await queue.join()
    for task in workers:
        task.cancel()
    await asyncio.gather(*workers, return_exceptions=True)

    return results


async def queue_is_bounded(maxsize: int) -> tuple[bool, int]:
    queue: asyncio.Queue[int] = asyncio.Queue(maxsize=maxsize)
    fitted = 0
    try:
        while True:
            # put_nowait raises rather than waiting, which makes the limit observable.
            queue.put_nowait(fitted)
            fitted += 1
    except asyncio.QueueFull:
        return True, fitted


async def drain_with_sentinel(items: list[int], worker_count: int) -> list[int]:
    queue: asyncio.Queue[int | None] = asyncio.Queue()
    results: list[int] = []

    async def worker() -> None:
        while True:
            item = await queue.get()
            if item is None:
                # A sentinel per worker lets each one return on its own, so the tasks
                # can be awaited normally instead of cancelled.
                return
            results.append(item)

    workers = [asyncio.create_task(worker()) for _ in range(worker_count)]
    for item in items:
        queue.put_nowait(item)
    for _ in range(worker_count):
        queue.put_nowait(None)

    await asyncio.gather(*workers)
    return results


async def worker_error_stops_nothing(items: list[int]) -> tuple[list[int], int]:
    queue: asyncio.Queue[int] = asyncio.Queue()
    successes: list[int] = []
    errors = 0

    async def worker() -> None:
        nonlocal errors
        while True:
            item = await queue.get()
            try:
                if item % 2 == 1:
                    raise ValueError(f"odd: {item}")
                successes.append(item)
            except ValueError:
                errors += 1
            finally:
                # Without this in finally, a raised handler would skip task_done() and
                # join() would wait forever for a count that never balances.
                queue.task_done()

    task = asyncio.create_task(worker())
    for item in items:
        queue.put_nowait(item)

    await queue.join()
    task.cancel()
    with contextlib.suppress(asyncio.CancelledError):
        await task

    return successes, errors


def run(coro: Awaitable[Any]) -> Any:
    return asyncio.run(coro)  # type: ignore[arg-type]
