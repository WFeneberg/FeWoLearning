"""Exercise 094 — an async worker pool with retries and backpressure (expert).

Goal:   Run many async tasks through a fixed-size pool of workers pulling from a
        bounded queue, retrying a failing task a few times before giving up on it
        — without one failure taking down the whole run, and without ever having
        more tasks "in flight" than `workers` allows.
Drills: `asyncio.Queue` as a work queue, a fixed number of worker coroutines
        competing for items via `asyncio.gather`, a bounded queue's `put` as the
        backpressure mechanism (a full queue makes the producer wait rather than
        buffering unboundedly), and retrying a coroutine (a coroutine object can
        only be awaited once — retrying means calling the async callable again for
        a fresh one, not re-awaiting the same object).
Passes: when `pytest exercises/04-expert/test_ex094_async_task_queue.py` is green.

Note:   ordering is asserted through recorded events and result positions, not wall
        clock time — same approach as `ex072`.
"""

import asyncio
from dataclasses import dataclass
from typing import Any, Awaitable, Callable


@dataclass
class TaskResult:
    """The outcome of running one task."""

    task_id: int
    value: Any = None
    error: BaseException | None = None
    attempts: int = 0


async def run_task_queue(
    tasks: list[Callable[[], Awaitable[Any]]],
    *,
    workers: int,
    max_retries: int = 0,
    queue_size: int = 0,
) -> list[TaskResult]:
    """Run every task in `tasks` through `workers` concurrent workers.

    `workers` must be positive — else ValueError. Feed the tasks (paired with their
    original index) through an `asyncio.Queue(maxsize=queue_size)` — `queue_size`
    of 0 means unbounded, matching `asyncio.Queue`'s own default; any positive
    value makes a producer's `put` block once the queue is full, which is the
    backpressure this exercise is named for.

    Each worker: pull one `(index, task)` pair, call `task()` (a fresh call — and
    therefore a fresh coroutine — every attempt), retrying up to `max_retries`
    additional times if it raises, then record a `TaskResult` with however many
    attempts it took. A task that still fails on its last attempt gets its
    exception recorded on `.error` rather than propagating — one task's failure
    must never stop the others, or crash `run_task_queue` itself.

    Return the results in the same order as `tasks`, regardless of the order the
    workers actually finished them in.
    """
    raise NotImplementedError


def run(coro: Awaitable[Any]) -> Any:
    """Run `coro` to completion and return its result."""
    raise NotImplementedError
