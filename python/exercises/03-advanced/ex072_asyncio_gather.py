"""Exercise 072 — asyncio.gather (advanced).

Goal:   Run awaitables concurrently and collect their results.
Drills: async def / await, gather preserving argument order, return_exceptions,
        the difference between concurrent and sequential awaiting, and why a plain
        `await` in a loop is not concurrency at all.
Passes: when `pytest exercises/03-advanced/test_ex072_asyncio_gather.py` is green.

Note:   nothing here sleeps on the wall clock for its correctness — ordering is
        asserted through recorded events, so the tests are deterministic.
"""

import asyncio
from typing import Any, Awaitable, Callable


async def fetch_all(fetchers: list[Callable[[], Awaitable[Any]]]) -> list[Any]:
    """Await every fetcher **concurrently** and return the results.

    gather returns results in the order of its *arguments*, never in completion
    order — that is what makes it safe to zip against the inputs.
    """
    raise NotImplementedError


async def fetch_all_sequential(fetchers: list[Callable[[], Awaitable[Any]]]) -> list[Any]:
    """Await the fetchers one after another.

    Exists so a test can show the difference: awaiting inside a loop serialises the
    work, no matter that the functions are async.
    """
    raise NotImplementedError


async def gather_with_errors(fetchers: list[Callable[[], Awaitable[Any]]]) -> list[Any]:
    """Await concurrently, returning exceptions in place of results.

    ``return_exceptions=True`` means one failure no longer cancels the rest, and the
    caller decides what to do with each entry.
    """
    raise NotImplementedError


async def first_error(fetchers: list[Callable[[], Awaitable[Any]]]) -> BaseException | None:
    """Await concurrently and return the first exception raised, or None.

    Without ``return_exceptions`` gather propagates the first failure immediately.
    """
    raise NotImplementedError


async def map_concurrently(
    func: Callable[[int], Awaitable[int]], values: list[int]
) -> list[int]:
    """Apply an async `func` to every value concurrently, results in input order."""
    raise NotImplementedError


async def bounded_gather(
    fetchers: list[Callable[[], Awaitable[Any]]], limit: int
) -> list[Any]:
    """Await concurrently but with at most `limit` running at once.

    gather alone has no limit; a Semaphore around each call provides one. A `limit`
    below 1 raises ValueError.
    """
    raise NotImplementedError


def run(coro: Awaitable[Any]) -> Any:
    """Run a coroutine to completion from synchronous code, via ``asyncio.run``."""
    raise NotImplementedError
