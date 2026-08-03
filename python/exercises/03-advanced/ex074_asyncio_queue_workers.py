"""Exercise 074 — asyncio.Queue and worker tasks (advanced).

Goal:   Build a producer/consumer pipeline with a bounded queue.
Drills: asyncio.Queue, put/get, task_done and join, worker tasks that outlive one
        item, shutting workers down cleanly, backpressure from maxsize.
Passes: when `pytest exercises/03-advanced/test_ex074_asyncio_queue_workers.py` is green.
"""

import asyncio
from typing import Any, Awaitable, Callable


async def process_all(
    items: list[int], worker_count: int, handler: Callable[[int], Awaitable[Any]]
) -> list[Any]:
    """Push every item through `worker_count` workers and return all results.

    Results come back in **completion** order, not input order — a queue makes no
    promises about which worker finishes first. Use ``queue.join()`` to wait for the
    work rather than awaiting the worker tasks, then cancel the workers, which would
    otherwise wait on an empty queue forever.

    A `worker_count` below 1 raises ValueError.
    """
    raise NotImplementedError


async def collect_ordered(
    items: list[int], worker_count: int, handler: Callable[[int], Awaitable[Any]]
) -> list[Any]:
    """Same, but results in **input** order.

    Enqueue ``(index, item)`` and write each result into a pre-sized list, so ordering
    survives out-of-order completion.
    """
    raise NotImplementedError


async def queue_is_bounded(maxsize: int) -> tuple[bool, int]:
    """Demonstrate backpressure.

    Create a Queue with `maxsize`, fill it with ``put_nowait`` until it refuses, and
    return ``(whether_it_refused, how_many_fit)``. A full queue raises QueueFull for
    ``put_nowait`` — the async ``put`` would instead wait, which is the backpressure.
    """
    raise NotImplementedError


async def drain_with_sentinel(items: list[int], worker_count: int) -> list[int]:
    """Shut workers down with a sentinel rather than by cancelling them.

    Each worker returns when it receives None. Push one sentinel per worker after the
    real items, then await the workers normally. Returns the processed items in
    completion order.
    """
    raise NotImplementedError


async def worker_error_stops_nothing(items: list[int]) -> tuple[list[int], int]:
    """Run one worker that raises on odd numbers, and keep going.

    Returns ``(successful_items, error_count)``. A handler failure must not leave the
    queue's ``join()`` hanging: ``task_done()`` has to be called in a ``finally``, or
    the count never balances.
    """
    raise NotImplementedError


def run(coro: Awaitable[Any]) -> Any:
    """Run a coroutine to completion."""
    raise NotImplementedError
